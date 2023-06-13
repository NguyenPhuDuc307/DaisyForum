import { Component, OnInit } from '@angular/core';
import { StatisticsService } from '@app/shared/services';
import { BaseComponent } from '@app/layout/base/base.component';

@Component({
  selector: 'app-monthly-new-comments',
  templateUrl: './monthly-new-comments.component.html',
  styleUrls: ['./monthly-new-comments.component.css']
})
export class MonthlyNewCommentsComponent extends BaseComponent implements OnInit {
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
    super('STATISTIC_MONTHLY_COMMENT');
    // Cấu hình options cho chart
    this.options = {
      responsive: true,
      maintainAspectRatio: false,
    };
  }
  ngOnInit() {
    super.ngOnInit();
    this.loadData();
  }
  loadData() {
    this.blockedPanel = true;
    this.statisticService.getMonthlyNewComments(this.year)
      .subscribe((response: any) => {
        this.totalItems = 0;
        this.items = response;
        response.forEach(element => {
          this.totalItems += element.numberOfComments;
        });

        // Cấu hình dữ liệu cho chart
        this.data = {
          labels: response.map(item => 'Tháng ' + item.month),
          datasets: [
            {
              label: 'Số bình luận theo tháng',
              backgroundColor: '#42A5F5',
              borderColor: '#1E88E5',
              data: response.map(item => item.numberOfComments)
            }
          ]
        };
        setTimeout(() => { this.blockedPanel = false; }, 100);
      }, error => {
        setTimeout(() => { this.blockedPanel = false; }, 100);
      });
  }
}
