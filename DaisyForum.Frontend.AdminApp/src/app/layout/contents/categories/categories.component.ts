import { Component, OnInit, OnDestroy } from '@angular/core';
import { MessageConstants } from '@app/shared/constants';
import { CategoriesDetailComponent } from './categories-detail/categories-detail.component';
import { CategoriesService, NotificationService, UtilitiesService } from '@app/shared/services';
import { Pagination, Category } from '@app/shared/models';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
import { BaseComponent } from '@app/layout/base/base.component';
import { TreeNode } from 'primeng/api/treenode';

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.css']
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
  public pageIndex = 1;
  public pageSize = 10;
  public pageDisplay = 10;
  public totalRecords: number;
  public keyword = '';
  // Role
  public items: any[];
  public selectedItems = [];
  constructor(private categoriesService: CategoriesService,
    private notificationService: NotificationService,
    private modalService: BsModalService,
    private utilitiesService: UtilitiesService) {
    super('CONTENT_CATEGORY');
  }

  ngOnInit(): void {
    super.ngOnInit();
    this.loadData();
  }


  loadData(selectedId = null) {
    this.blockedPanel = true;
    this.categoriesService.getAll()
      .subscribe((response: any) => {
        const functionTree = this.utilitiesService.UnflatteringForTree(response);
        this.items = <TreeNode[]>functionTree;
        if (this.selectedItems.length === 0 && this.items.length > 0) {
          this.selectedItems.push(this.items[0]);
        }
        // Nếu có là sửa thì chọn selection theo Id
        if (selectedId != null && this.items.length > 0) {
          this.selectedItems = this.items.filter(x => x.data.id == selectedId);
        }

        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }, error => {
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      });
  }

  pageChanged(event: any): void {
    this.pageIndex = event.page + 1;
    this.pageSize = event.rows;
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
      entityId: this.selectedItems[0].id
    };
    this.bsModalRef = this.modalService.show(CategoriesDetailComponent,
      {
        initialState: initialState,
        class: 'modal-lg',
        backdrop: 'static'
      });

    this.subscription.add(this.bsModalRef.content.savedEvent.subscribe((response) => {
      this.bsModalRef.hide();
      this.loadData(response.id);
    }));
  }

  deleteItems() {
    if (this.selectedItems.length === 0) {
      this.notificationService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
      return;
    }
    const id = this.selectedItems[0].id;
    this.notificationService.showConfirmation(MessageConstants.CONFIRM_DELETE_MSG,
      () => this.deleteItemsConfirm(id));
  }

  deleteItemsConfirm(id: string) {
    this.blockedPanel = true;
    this.categoriesService.delete(id).subscribe(() => {
      this.notificationService.showSuccess(MessageConstants.DELETED_OK_MSG);
      this.loadData();
      this.selectedItems = [];
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    }, error => {
      setTimeout(() => { this.blockedPanel = false; }, 1000);
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
