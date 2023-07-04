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
  public data: any;
  public options: any;

  constructor(
    private statisticService: StatisticsService) {
    super('STATISTIC_MONTHLY_NEW_KNOWLEDGE_BASE');

    // Cấu hình options cho chart
    this.options = {
      responsive: true,
      maintainAspectRatio: true,
    };
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
        // Cấu hình dữ liệu cho chart
        this.data = {
          labels: response.map(item => 'Tháng ' + item.month),
          datasets: [
            {
              label: 'Số bài viết theo tháng',
              type: 'line',
              borderColor: '#ff9f42',
              borderWidth: 2,
              data: response.map(item => item.numberOfNewKnowledgeBases),
              lineTension: 0.4
            },
            {
              label: 'Số bài viết theo tháng',
              backgroundColor: '#7254f3',
              borderColor: '#1E88E5',
              data: response.map(item => item.numberOfNewKnowledgeBases)
            }
          ]
        };
        this.totalItems = response.reduce((total, item) => total += item.numberOfNewKnowledgeBases, 0);
        setTimeout(() => { this.blockedPanel = false; }, 100);
      }, error => {
        setTimeout(() => { this.blockedPanel = false; }, 100);
      });
  }
}