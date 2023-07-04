import { Component, OnInit } from '@angular/core';
import { StatisticsService } from '@app/shared/services';
import { BaseComponent } from '@app/layout/base/base.component';

@Component({
  selector: 'app-monthly-new-members',
  templateUrl: './monthly-new-members.component.html',
  styleUrls: ['./monthly-new-members.component.css']
})
export class MonthlyNewMembersComponent extends BaseComponent implements OnInit {

  // Default
  public blockedPanel = false;
  // Customer Receivable
  public items: any[];
  public year: number = new Date().getFullYear();
  public totalItems = 0;

  // Thêm các biến để cấu hình chart
  public data: any;
  public options: any;

  constructor(
    private statisticService: StatisticsService) {
    super('STATISTIC_MONTHLY_NEW_MEMBER');
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
    this.statisticService.getMonthlyNewMembers(this.year)
      .subscribe((response: any) => {
        this.totalItems = 0;
        this.items = response;
        response.forEach(element => {
          this.totalItems += element.numberOfRegisters;
        });
        // Cấu hình dữ liệu cho chart
        this.data = {
          labels: response.map(item => 'Tháng ' + item.month),
          datasets: [
            {
              label: 'Số thành viên mới theo tháng',
              type: 'line',
              borderColor: '#ff9f42',
              borderWidth: 2,
              data: response.map(item => item.numberOfRegisters),
              lineTension: 0.4
            },
            {
              label: 'Số thành viên mới theo tháng',
              backgroundColor: '#7254f3',
              borderColor: '#1E88E5',
              data: response.map(item => item.numberOfRegisters)
            }
          ]
        };
        setTimeout(() => { this.blockedPanel = false; }, 100);
      }, error => {
        setTimeout(() => { this.blockedPanel = false; }, 100);
      });
  }
}