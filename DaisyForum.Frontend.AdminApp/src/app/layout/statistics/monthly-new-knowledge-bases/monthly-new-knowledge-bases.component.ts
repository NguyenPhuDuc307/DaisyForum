import { Component, OnInit } from '@angular/core';
import { StatisticsService } from '@app/shared/services';
import { BaseComponent } from '@app/layout/base/base.component';

@Component({
  selector: 'app-monthly-new-kbs',
  templateUrl: './monthly-new-knowledge-bases.component.html',
  styleUrls: ['./monthly-new-knowledge-bases.component.css']
})
export class MonthlyNewKbsComponent extends BaseComponent implements OnInit {
  // Default
  public blockedPanel = false;
  // Customer Receivable
  public items: any[];
  public year: number = new Date().getFullYear();
  public totalItems = 0;
  constructor(
    private statisticService: StatisticsService) {
    super('STATISTIC_MONTHLY_NEW_KNOWLEDGE_BASE');

  }
  ngOnInit() {
    super.ngOnInit();
    this.loadData();
  }
  loadData() {
    this.blockedPanel = true;
    this.statisticService.getMonthlyNewKbs(this.year)
      .subscribe((response: any) => {
        this.totalItems = 0;
        this.items = response;
        response.forEach(element => {
          this.totalItems += element.numberOfNewKnowledgeBases;
        });
        setTimeout(() => { this.blockedPanel = false; }, 100);
      }, error => {
        setTimeout(() => { this.blockedPanel = false; }, 100);
      });
  }

}
