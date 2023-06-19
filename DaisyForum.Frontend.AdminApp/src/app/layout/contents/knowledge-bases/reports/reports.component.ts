import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { BaseComponent } from '@app/layout/base/base.component';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ActivatedRoute } from '@angular/router';
import { NotificationService, ReportsService } from '@app/shared/services';
import { Pagination, Report } from '@app/shared/models';
import { MessageConstants } from '@app/shared/constants';
import { ReportsDetailComponent } from '../reports-detail/reports-detail.component';

@Component({
    selector: 'app-reports',
    templateUrl: './reports.component.html',
    styleUrls: ['./reports.component.scss']
})
export class ReportsComponent extends BaseComponent implements OnInit, OnDestroy {
    private subscription = new Subscription();
    // Default
    public bsModalRef: BsModalRef;
    public blockedPanel = false;
    public entityId: number;
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
    constructor(private reportsService: ReportsService,
        private notificationService: NotificationService,
        private activeRoute: ActivatedRoute,
        private modalService: BsModalService) {
        super('CONTENT_REPORT');
    }

    ngOnInit(): void {
        super.ngOnInit();
        this.subscription.add(this.activeRoute.params.subscribe(params => {
            this.entityId = params['knowledgeBaseId'];
        }));
        this.loadData();
    }

    loadData(selectedId = null) {
        this.blockedPanel = true;
        this.subscription.add(this.reportsService.getAllPaging(this.entityId, this.keyword, this.pageIndex, this.pageSize)
            .subscribe((response: Pagination<Report>) => {
                this.processLoadData(selectedId, response);
                setTimeout(() => { this.blockedPanel = false; }, 100);
            }, error => {
                setTimeout(() => { this.blockedPanel = false; }, 100);
            }));
    }
    private processLoadData(selectedId = null, response: Pagination<Report>) {
        const now = new Date(); // Lấy thời điểm hiện tại
        const reports = response.items.map(report => {
            const timeDiff = now.getTime() - new Date(report.createDate).getTime();
            const timeDiffString = this.getTimeDiffString(timeDiff);
            return {
                ...report,
                timeDiff: timeDiffString
            };
        });
        this.items = reports;
        this.pageIndex = this.pageIndex;
        this.pageSize = this.pageSize;
        this.totalRecords = response.totalRecords;
        if (this.selectedItems.length === 0 && this.items.length > 0) {
            this.selectedItems.push(this.items[0]);
        }
        if (selectedId != null && this.items.length > 0) {
            this.selectedItems = this.items.filter(x => x.Id === selectedId);
        }
    }
    pageChanged(event: any): void {
        this.pageIndex = event.page + 1;
        this.pageSize = event.rows;
        this.loadData();
    }

    showDetailModel() {
        if (this.selectedItems.length === 0) {
            this.notificationService.showError(MessageConstants.NOT_CHOOSE_ANY_RECORD);
            return;
        }
        const initialState = {
            reportId: this.selectedItems[0].id,
            knowledgeBaseId: this.selectedItems[0].knowledgeBaseId
        };
        this.bsModalRef = this.modalService.show(ReportsDetailComponent,
            {
                initialState: initialState,
                class: 'modal-lg',
                backdrop: 'static'
            });
    }

    private getTimeDiffString(timeDiff: number): string {
        const seconds = Math.floor(timeDiff / 1000);
        if (seconds < 60) {
            return 'khoảng vài giây trước';
        }
        const minutes = Math.floor(seconds / 60);
        if (minutes < 60) {
            return `khoảng ${minutes} phút trước`;
        }
        const hours = Math.floor(minutes / 60);
        if (hours < 24) {
            return `khoảng ${hours} giờ trước`;
        }
        const days = Math.floor(hours / 24);
        if (days < 7) {
            return `khoảng ${days} ngày trước`;
        }
        const weeks = Math.floor(days / 7);
        return `khoảng ${weeks} tuần trước`;
    }

    deleteItems() {
        const reportId = this.selectedItems[0].id;
        const knowledgeBaseId = this.selectedItems[0].knowledgeBaseId;
        this.notificationService.showConfirmation(MessageConstants.CONFIRM_DELETE_MSG,
            () => this.deleteItemsConfirm(knowledgeBaseId, reportId));
    }
    deleteItemsConfirm(knowledgeBaseId, reportId) {
        this.blockedPanel = true;
        this.subscription.add(this.reportsService.delete(knowledgeBaseId, reportId).subscribe(() => {
            this.notificationService.showSuccess(MessageConstants.DELETED_OK_MSG);
            this.loadData();
            this.selectedItems = [];
            setTimeout(() => { this.blockedPanel = false; }, 100);
        }, error => {
            setTimeout(() => { this.blockedPanel = false; }, 100);
        }));
    }

    ngOnDestroy(): void {
        this.subscription.unsubscribe();
    }
}