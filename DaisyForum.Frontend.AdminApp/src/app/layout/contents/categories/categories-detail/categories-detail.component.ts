import { Component, OnInit, EventEmitter, OnDestroy } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CategoriesService, NotificationService, UtilitiesService } from '@app/shared/services';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { MessageConstants } from '@app/shared/constants';
import { SelectItem } from 'primeng/api/selectitem';
import { Category } from '@app/shared/models';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-categories-detail',
  templateUrl: './categories-detail.component.html',
  styleUrls: ['./categories-detail.component.scss'],
  providers: [MessageService]
})
export class CategoriesDetailComponent implements OnInit, OnDestroy {

  constructor(
    public bsModalRef: BsModalRef,
    private categoriesService: CategoriesService,
    private notificationService: NotificationService,
    private utilitiesService: UtilitiesService,
    private fb: FormBuilder,
    private messageService: MessageService) {
  }

  private subscription = new Subscription();
  public entityForm: FormGroup;
  public dialogTitle: string;
  private savedEvent: EventEmitter<any> = new EventEmitter();
  public entityId: number;
  public btnDisabled = false;
  public blockedPanel = false;
  public categories: SelectItem[] = [];

  // Validate
  validation_messages = {
    'id': [
      { type: 'required', message: 'Trường này bắt buộc' },
      { type: 'maxlength', message: 'Bạn không được nhập quá 25 kí tự' }
    ],
    'name': [
      { type: 'required', message: 'Trường này bắt buộc' },
      { type: 'maxlength', message: 'Bạn không được nhập quá 30 kí tự' }
    ],
    'sortOrder': [
      { type: 'required', message: 'Trường này bắt buộc' },
    ]
  };

  ngOnInit() {
    this.entityForm = this.fb.group({
      'parentId': new FormControl(null),
      'name': new FormControl('', Validators.compose([
        Validators.required,
        Validators.maxLength(50)
      ])),
      'seoAlias': new FormControl('', Validators.compose([
        Validators.required
      ])),
      'seoDescription': new FormControl(''),
      'sortOrder': new FormControl(0, Validators.required)
    });
    this.subscription.add(this.categoriesService.getAll()
      .subscribe((response: Category[]) => {
        response.forEach(element => {
          this.categories.push({
            value: element.id,
            label: element.name
          });
        });
        if (this.entityId) {
          this.dialogTitle = 'Cập nhật';
          this.loadFormDetails(this.entityId);
        } else {
          this.dialogTitle = 'Thêm mới';
        }
      }));
  }
  public generateSeoAlias() {
    const seoAlias = this.utilitiesService.MakeSeoTitle(this.entityForm.controls['name'].value);
    this.entityForm.controls['seoAlias'].setValue(seoAlias);
  }
  private loadFormDetails(id: any) {
    this.blockedPanel = true;
    this.subscription.add(this.categoriesService.getDetail(id).subscribe((response: any) => {
      this.entityForm.setValue({
        parentId: response.parentId,
        name: response.name,
        seoAlias: response.seoAlias,
        seoDescription: response.seoDescription,
        sortOrder: response.sortOrder
      });
      setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 100);
    }, error => {
      setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 100);
    }));
  }
  public saveChange() {
    this.btnDisabled = true;
    this.blockedPanel = true;
    if (this.entityId) {
      this.subscription.add(this.categoriesService.update(this.entityId, this.entityForm.getRawValue())
        .subscribe(() => {
          this.savedEvent.emit(this.entityForm.value);
          this.notificationService.showSuccess(MessageConstants.UPDATED_OK_MSG);
          // this.messageService.add({ severity: 'success', summary: '', detail: MessageConstants.UPDATED_OK_MSG });
          this.btnDisabled = false;
          setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 100);
        }, error => {
          setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 100);
        }));
    } else {
      this.subscription.add(this.categoriesService.add(this.entityForm.getRawValue())
        .subscribe(() => {
          this.savedEvent.emit(this.entityForm.value);
          this.notificationService.showSuccess(MessageConstants.CREATED_OK_MSG);
          this.btnDisabled = false;
          setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 100);
        }, error => {
          setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 100);
        }));
    }
  }
  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
