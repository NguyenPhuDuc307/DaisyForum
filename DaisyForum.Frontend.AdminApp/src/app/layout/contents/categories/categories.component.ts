import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { MessageConstants } from '@app/shared/constants';
import { CategoriesDetailComponent } from './categories-detail/categories-detail.component';
import { CategoriesService, NotificationService, UtilitiesService } from '@app/shared/services';
import { Category } from '@app/shared/models';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { BaseComponent } from '@app/layout/base/base.component';
import { TreeNode } from 'primeng/api/treenode';
import { Paginator } from 'primeng/paginator';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.css'],
  providers: [MessageService]
})
export class CategoriesComponent extends BaseComponent implements OnInit, OnDestroy {

  private subscription = new Subscription();
  public screenTitle: string;
  // Default
  public bsModalRef: BsModalRef;
  public blockedPanel = false;
  /**
   * Paging
   */
  public page: number = 0;
  public rows: number = 10;
  public totalRecords: number;
  public keyword = '';
  // Role
  public items: TreeNode[];
  public selectedItems: TreeNode[];
  public selection: TreeNode[] = []; // thêm biến selection

  @ViewChild('paginator') paginator: Paginator;

  constructor(private categoriesService: CategoriesService,
    private notificationService: NotificationService,
    private modalService: BsModalService,
    private utilitiesService: UtilitiesService,
    private messageService: MessageService) {
    super('CONTENT_CATEGORY');
    this.selectedItems = [];
  }

  ngOnInit(): void {
    super.ngOnInit();
    this.loadData();
  }

  loadData() {
    this.blockedPanel = true;
    this.categoriesService.getAllPaging(this.keyword, this.page, this.rows)
      .subscribe((response: any) => {
        const categories: Category[] = response.items;
        const treeNodes: TreeNode[] = categories.map(category => this.utilitiesService.convertToTreeNode(category));
        this.items = treeNodes;

        if (this.selectedItems.length === 0 && this.items.length > 0) {
          this.selectedItems.push(this.items[0]);
        }

        this.totalRecords = response.totalRecords;
        setTimeout(() => { this.blockedPanel = false; }, 100);
      }, error => {
        setTimeout(() => { this.blockedPanel = false; }, 100);
      });
  }

  nodeSelect(event) {
    this.selection = [];
    this.selection.push(event.node);
    //this.propagateSelectionDown(event.node, true);
    this.selectedItems = this.selection;
  }

  nodeUnSelect(event) {
    const index = this.selection.findIndex(node => node.key === event.node.key);
    if (index !== -1) {
      this.selection.splice(index, 1);
      // this.propagateSelectionDown(event.node, false);
      this.selectedItems = this.selection;
    }
  }

  pageChanged(event: any): void {
    this.page = event.page;
    this.rows = event.rows;
    this.loadData();
  }

  showAddModal() {
    this.bsModalRef = this.modalService.show(CategoriesDetailComponent,
      {
        class: 'modal-lg',
        backdrop: 'static'
      });
    this.bsModalRef.content.savedEvent.subscribe((response) => {
      this.bsModalRef.hide();
      this.loadData();
      this.selectedItems = [];
    });
  }

  showEditModal() {
    if (this.selectedItems.length === 0) {
      this.notificationService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
      return;
    }
    const initialState = {
      entityId: this.selectedItems[0]?.data?.id
    };
    this.bsModalRef = this.modalService.show(CategoriesDetailComponent,
      {
        initialState: initialState,
        class: 'modal-lg',
        backdrop: 'static'
      });

    this.subscription.add(this.bsModalRef.content.savedEvent.subscribe((response) => {
      this.bsModalRef.hide();
      this.loadData();
    }));
  }

  deleteItems() {
    if (this.selectedItems.length === 0) {
      this.notificationService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
      return;
    }
    const id = this.selectedItems[0].data.id;
    this.notificationService.showConfirmation(MessageConstants.CONFIRM_DELETE_MSG,
      () => this.deleteItemsConfirm(id));
  }

  deleteItemsConfirm(id: string) {
    this.blockedPanel = true;
    this.categoriesService.delete(id).subscribe(() => {
      this.notificationService.showSuccess(MessageConstants.DELETED_OK_MSG);
      this.loadData();
      this.selectedItems = [];
      setTimeout(() => { this.blockedPanel = false; }, 100);
    }, error => {
      setTimeout(() => { this.blockedPanel = false; }, 100);
    });
  }

  /**
   * Đoạn code để sửa lỗi
   * PhƯơng thức nodeSelect() và nodeUnSelect() trước khi sử dụng Paginator
   */
  propagateSelectionDown(node: TreeNode, isSelected: boolean) {
    node.children?.forEach(childNode => {
      this.propagateSelectionDown(childNode, isSelected);
      if (isSelected) {
        this.selection.push(childNode);
      } else {
        const index = this.selection.findIndex(node => node.key === childNode.key);
        if (index !== -1) {
          this.selection.splice(index, 1);
        }
      }
    });
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

}