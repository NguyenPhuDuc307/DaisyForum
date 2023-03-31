import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MonthlyCommentsComponent } from './monthly-comments/monthly-comments.component';
import { MonthlyNewKnowledgeBasesComponent } from './monthly-new-knowledge-bases/monthly-new-knowledge-bases.component';
import { MonthlyNewMembersComponent } from './monthly-new-members/monthly-new-members.component';

const routes: Routes = [
    {
        path: '',
        component: MonthlyNewKnowledgeBasesComponent

    },
    {
        path: 'monthly-new-knowledge-bases',
        component: MonthlyNewKnowledgeBasesComponent
    },
    {
        path: 'monthly-comments',
        component: MonthlyCommentsComponent
    },
    {
        path: 'monthly-new-members',
        component: MonthlyNewMembersComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class StatisticsRoutingModule {}
