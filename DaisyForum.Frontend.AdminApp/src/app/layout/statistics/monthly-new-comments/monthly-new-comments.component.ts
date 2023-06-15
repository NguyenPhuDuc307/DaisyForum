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
      maintainAspectRatio: true,
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
              label: 'Tổng số bình luận theo tháng',
              type: 'line',
              borderColor: '#a9bbcb',
              borderWidth: 2,
              data: response.map(item => item.numberOfComments),
              lineTension: 0.4
            },
            {
              label: 'Tổng số bình luận theo tháng',
              backgroundColor: '#7254f3',
              borderColor: '#1E88E5',
              data: response.map(item => item.numberOfComments)
            },
            {
              label: 'Số bình luận tích cực theo tháng',
              backgroundColor: '#29c76f',
              borderColor: '#FB8C00',
              data: response.map(item => item.numberOfPositiveComments)
            },
            {
              label: 'Số bình luận trung tính theo tháng',
              backgroundColor: '#c6d1dd',
              borderColor: '#FB8C00',
              data: response.map(item => item.numberOfNeutralComments)
            },
            {
              label: 'Số bình luận tiêu cực theo tháng',
              backgroundColor: '#ff9f42',
              borderColor: '#FB8C00',
              data: response.map(item => item.numberOfNegativeComments)
            }
          ]
        };
        setTimeout(() => { this.blockedPanel = false; }, 100);
      }, error => {
        setTimeout(() => { this.blockedPanel = false; }, 100);
      });
  }
}