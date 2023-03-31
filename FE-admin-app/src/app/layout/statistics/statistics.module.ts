import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MonthlyCommentsComponent } from './monthly-comments/monthly-comments.component';
import { MonthlyNewKnowledgeBasesComponent } from './monthly-new-knowledge-bases/monthly-new-knowledge-bases.component';
import { MonthlyNewMembersComponent } from './monthly-new-members/monthly-new-members.component';
import { StatisticsRoutingModule } from './statistics-routing.module';



@NgModule({
  declarations: [
    MonthlyCommentsComponent,
    MonthlyNewKnowledgeBasesComponent,
    MonthlyNewMembersComponent
  ],
  imports: [
    CommonModule,
    StatisticsRoutingModule
  ]
})
export class StatisticsModule { }
