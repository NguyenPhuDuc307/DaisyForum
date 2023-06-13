import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommentsService } from '@app/shared/services';
import { Subscription } from 'rxjs';
import { Comment } from '@app/shared/models';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
    selector: 'app-comments-detail',
    templateUrl: './comments-detail.component.html',
    styleUrls: ['./comments-detail.component.scss']
})
export class CommentsDetailComponent implements OnInit, OnDestroy {
    constructor(
        public bsModalRef: BsModalRef,
        private commentsService: CommentsService) {
    }

    private subscription = new Subscription();
    public dialogTitle: string;
    public knowledgeBaseId: number;
    public commentId: number;
    public btnDisabled = false;
    public blockedPanel = false;
    public comment: Comment;

    ngOnInit() {
        if (this.commentId) {
            this.loadFormDetails(this.knowledgeBaseId, this.commentId);
        }
    }
    private loadFormDetails(knowledgeBaseId, commentId) {
        this.blockedPanel = true;
        this.subscription.add(this.commentsService.getDetail(knowledgeBaseId, commentId)
            .subscribe((response: Comment) => {
                this.comment = mapNavigation(response);
                setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 1000);
            }, error => {
                setTimeout(() => { this.blockedPanel = false; this.btnDisabled = false; }, 1000);
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
