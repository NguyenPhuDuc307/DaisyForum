import { Component, OnInit } from '@angular/core';
import { routerTransition } from '../../router.animations';
import { StatisticsService } from '@app/shared/services';
import { BaseComponent } from '../base/base.component';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
    animations: [routerTransition()]
})
export class DashboardComponent extends BaseComponent implements OnInit {
    // Default
    public blockedPanel = false;
    public itemComments: any[];
    public itemMembers: any[];
    public itemKbs: any[];
    public year: number = new Date().getFullYear();
    public totalComments = 0;
    public totalMembers = 0;
    public totalKbs = 0;
    public dataComment: any;
    public optionComments: any;
    public dataMember: any;
    public optionMembers: any;
    public dataKb: any;
    public optionKbs: any;

    constructor(private statisticService: StatisticsService) {
        super('DASHBOARD');
        this.optionComments = {
            responsive: true,
            maintainAspectRatio: true,
        };
        this.optionMembers = {
            responsive: true,
            maintainAspectRatio: true,
        };
        this.optionKbs = {
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
                this.itemComments = response;
                this.totalComments = response.reduce((total, item) => total += item.numberOfComments, 0);

                // Cấu hình dữ liệu cho chart
                this.dataComment = {
                    labels: response.map(item => 'Tháng ' + item.month),
                    datasets: [
                        {
                            label: 'Số bình luận',
                            backgroundColor: ['#7254F3', '#29C76F', '#FF9F42', '#5486F3', '#3eC9D6', '#A1C30D', '#EA5455'],
                            data: response.map(item => item.numberOfComments)
                        }
                    ]
                };
                setTimeout(() => { this.blockedPanel = false; }, 100);
            }, error => {
                setTimeout(() => { this.blockedPanel = false; }, 100);
            });

        this.statisticService.getMonthlyNewKbs(this.year)
            .subscribe((response: any) => {
                this.itemKbs = response;
                this.totalKbs = response.reduce((total, item) => total += item.numberOfNewKnowledgeBases, 0);

                // Cấu hình dữ liệu cho chart
                this.dataKb = {
                    labels: response.map(item => 'Tháng ' + item.month),
                    datasets: [
                        {
                            label: 'Số bài viết theo tháng',
                            backgroundColor: ['#7254F3', '#29C76F', '#FF9F42', '#5486F3', '#3eC9D6', '#A1C30D', '#EA5455'],
                            data: response.map(item => item.numberOfNewKnowledgeBases)
                        }
                    ]
                };
                setTimeout(() => { this.blockedPanel = false; }, 100);
            }, error => {
                setTimeout(() => { this.blockedPanel = false; }, 100);
            });

        this.statisticService.getMonthlyNewMembers(this.year)
            .subscribe((response: any) => {
                this.itemMembers = response;
                this.totalMembers = response.reduce((total, item) => total += item.numberOfRegisters, 0);

                // Cấu hình dữ liệu cho chart
                this.dataMember = {
                    labels: response.map(item => 'Tháng ' + item.month),
                    datasets: [
                        {
                            label: 'Số thành viên mới theo tháng',
                            backgroundColor: ['#7254F3', '#29C76F', '#FF9F42', '#5486F3', '#3eC9D6', '#A1C30D', '#EA5455'],
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