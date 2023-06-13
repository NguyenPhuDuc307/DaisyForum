import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Pagination, Comment } from '@app/shared/models';
import { NotificationService, CommentsService } from '@app/shared/services';
import { MessageConstants } from '@app/shared/constants';
import { CommentsDetailComponent } from '../comments-detail/comments-detail.component';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-comments',
    templateUrl: './comments.component.html',
    styleUrls: ['./comments.component.scss']
})
export class CommentsComponent implements OnInit, OnDestroy {
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
    constructor(private commentsService: CommentsService,
        private notificationService: NotificationService,
        private activeRoute: ActivatedRoute,
        private modalService: BsModalService) { }

    ngOnInit(): void {
        this.subscription.add(this.activeRoute.params.subscribe(params => {
            this.entityId = params['knowledgeBaseId'];
        }));
        this.loadData();
    }

    loadData(selectedId = null) {
        // this.blockedPanel = true;
        this.subscription.add(this.commentsService.getAllPaging(this.entityId, this.keyword, this.pageIndex, this.pageSize)
            .subscribe((response: Pagination<Comment>) => {
                this.processLoadData(selectedId, response);
                setTimeout(() => { this.blockedPanel = false; }, 100);
            }, error => {
                setTimeout(() => { this.blockedPanel = false; }, 100);
            }));
    }

    private processLoadData(selectedId = null, response: Pagination<Comment>) {
        const now = new Date(); // Lấy thời điểm hiện tại
        const comments = response.items.map(comment => {
            const timeDiff = now.getTime() - new Date(comment.createDate).getTime();
            const timeDiffString = this.getTimeDiffString(timeDiff);
            return {
                ...mapNavigation(comment),
                timeDiff: timeDiffString
            };
        });
        this.items = comments;
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
            entityId: this.selectedItems[0].id,
            commentId: this.selectedItems[0].id,
            knowledgeBaseId: this.entityId
        };
        this.bsModalRef = this.modalService.show(CommentsDetailComponent,
            {
                initialState: initialState,
                class: 'modal-lg',
                backdrop: 'static'
            });
    }

    deleteItems() {
        const commentId = this.selectedItems[0].id;
        const knowledgeBaseId = this.selectedItems[0].knowledgeBaseId;
        this.notificationService.showConfirmation(MessageConstants.CONFIRM_DELETE_MSG,
            () => this.deleteItemsConfirm(commentId, knowledgeBaseId));
    }
    deleteItemsConfirm(commentId, knowledgeBaseId) {
        // this.blockedPanel = true;
        this.subscription.add(this.commentsService.delete(knowledgeBaseId, commentId).subscribe(() => {
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

function mapNavigation(comment: Comment): Comment {
    if (comment.navigationScore > 0.3) {
        comment.navigation = "Tích cực";
    } else if (comment.navigationScore < -0.3) {
        comment.navigation = "Tiêu cực";
    } else {
        comment.navigation = "Trung tính";
    }
    return comment;
}

