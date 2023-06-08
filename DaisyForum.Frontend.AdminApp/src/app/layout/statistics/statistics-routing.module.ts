import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MonthlyNewKbsComponent } from './monthly-new-knowledge-bases/monthly-new-knowledge-bases.component';
import { MonthlyNewMembersComponent } from './monthly-new-members/monthly-new-members.component';
import { MonthlyNewCommentsComponent } from './monthly-new-comments/monthly-new-comments.component';
import { AuthGuard } from '@app/shared';

const routes: Routes = [
    {
        path: '',
        component: MonthlyNewKbsComponent
    },
    {
        path: 'monthly-new-knowledge-bases',
        component: MonthlyNewKbsComponent,
        data: {
            functionCode: 'STATISTIC_MONTHLY_NEW_KNOWLEDGE_BASE'
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'monthly-registers',
        component: MonthlyNewMembersComponent,
        data: {
            functionCode: 'STATISTIC_MONTHLY_NEW_MEMBER'
        },
        canActivate: [AuthGuard]
    },
    {
        path: 'monthly-comments',
        component: MonthlyNewCommentsComponent,
        data: {
            functionCode: 'STATISTIC_MONTHLY_COMMENT'
        },
        canActivate: [AuthGuard]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class StatisticsRoutingModule { }
