import { Component, OnInit, OnDestroy } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { NotificationService, UtilitiesService, CommandsService, RolesService } from '@app/shared/services';
import { SystemConstants, MessageConstants } from '@app/shared/constants';
import { TreeNode } from 'primeng/api/treenode';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionUpdateRequest } from '@app/shared/models';
import { Permission } from '@app/shared/models/permission.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-permissions',
  templateUrl: './permissions.component.html',
  styleUrls: ['./permissions.component.css']
})
export class PermissionsComponent implements OnInit, OnDestroy {

  private subscription = new Subscription();

  public bsModalRef: BsModalRef;
  public blockedPanel = false;

  public functions: any[];
  public flattenFunctions: any[] = [];
  public selectedRole: any = {
    id: null
  };
  public roles: any[] = [];
  public commands: any[] = [];

  public selectedViews: string[] = [];
  public selectedCreates: string[] = [];
  public selectedUpdates: string[] = [];
  public selectedDeletes: string[] = [];
  public selectedApproves: string[] = [];

  public isSelectedAllViews = false;
  public isSelectedAllCreates = false;
  public isSelectedAllUpdates = false;
  public isSelectedAllDeletes = false;
  public isSelectedAllApproves = false;

  constructor(

    private permissionsService: PermissionsService,
    private rolesService: RolesService,
    private _notificationService: NotificationService,
    private _utilityService: UtilitiesService) {
  }


  ngOnInit() {
    this.loadAllRoles();
    this.loadData(this.selectedRole.id);
  }

  changeRole($event: any) {
    if ($event.value != null) {
      this.loadData($event.value.id);
    } else {
      this.functions = [];
    }
  }
  public reloadData() {
    this.loadData(this.selectedRole.id);
  }
  public savePermission() {
    if (this.selectedRole.id == null) {
      this._notificationService.showError('Bạn chưa chọn nhóm quyền.');
      return;
    }
    const listPermissions: Permission[] = [];
    this.selectedCreates.forEach(element => {
      listPermissions.push({
        functionId: element,
        roleId: this.selectedRole.id,
        commandId: SystemConstants.CREATE_ACTION
      });
    });
    this.selectedUpdates.forEach(element => {
      listPermissions.push({
        functionId: element,
        roleId: this.selectedRole.id,
        commandId: SystemConstants.UPDATE_ACTION
      });
    });
    this.selectedDeletes.forEach(element => {
      listPermissions.push({
        functionId: element,
        roleId: this.selectedRole.id,
        commandId: SystemConstants.DELETE_ACTION
      });
    });
    this.selectedViews.forEach(element => {
      listPermissions.push({
        functionId: element,
        roleId: this.selectedRole.id,
        commandId: SystemConstants.VIEW_ACTION
      });
    });

    this.selectedApproves.forEach(element => {
      listPermissions.push({
        functionId: element,
        roleId: this.selectedRole.id,
        commandId: SystemConstants.APPROVE_ACTION
      });
    });
    const permissionsUpdateRequest = new PermissionUpdateRequest();
    permissionsUpdateRequest.permissions = listPermissions;
    this.subscription.add(this.permissionsService.save(this.selectedRole.id, permissionsUpdateRequest)
      .subscribe(() => {
        this._notificationService.showSuccess(MessageConstants.UPDATED_OK_MSG);

        setTimeout(() => { this.blockedPanel = false; }, 100);
      }, error => {
        setTimeout(() => { this.blockedPanel = false; }, 100);
      }));
  }
  loadData(roleId) {
    if (roleId != null) {
      this.blockedPanel = true;
      this.subscription.add(this.permissionsService.getFunctionWithCommands()
        .subscribe((response: any) => {
          const unflattering = this._utilityService.UnflatteringForTree(response);
          this.functions = <TreeNode[]>unflattering;
          this.flattenFunctions = response;
          this.fillPermissions(roleId);
          setTimeout(() => { this.blockedPanel = false; }, 100);
        }, error => {
          setTimeout(() => { this.blockedPanel = false; }, 100);
        }));
    }

  }

  checkChanged(checked: boolean, commandId: string, functionId: string, parentId: string) {
    if (commandId === SystemConstants.VIEW_ACTION) {
      if (checked) {
        // Nếu checkbox chưa được chọn thì thêm vào các mảng lựa chọn
        if (!this.selectedViews.includes(functionId)) {
          this.selectedViews.push(functionId);
        }
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedViews.push(...childFunctions);
        } else {
          if (!this.selectedViews.includes(parentId)) {
            this.selectedViews.push(parentId);
          }
        }
      } else {
        // Nếu checkbox đã được chọn thì bỏ chọn
        this.selectedViews = this.selectedViews.filter(x => x !== functionId);
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedViews = this.selectedViews.filter(el => !childFunctions.includes(el));
        }
      }
    } else if (commandId === SystemConstants.CREATE_ACTION) {
      if (checked) {
        // Nếu checkbox chưa được chọn thì thêm vào các mảng lựa chọn
        if (!this.selectedCreates.includes(functionId)) {
          this.selectedCreates.push(functionId);
        }
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedCreates.push(...childFunctions);
        } else {
          if (!this.selectedCreates.includes(parentId)) {
            this.selectedCreates.push(parentId);
          }
        }
      } else {
        // Nếu checkbox đã được chọn thì bỏ chọn
        this.selectedCreates = this.selectedCreates.filter(x => x !== functionId);
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedCreates = this.selectedCreates.filter(el => !childFunctions.includes(el));
        }
      }
    } else if (commandId === SystemConstants.UPDATE_ACTION) {
      if (checked) {
        // Nếu checkbox chưa được chọn thì thêm vào các mảng lựa chọn
        if (!this.selectedUpdates.includes(functionId)) {
          this.selectedUpdates.push(functionId);
        }
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedUpdates.push(...childFunctions);
        } else {
          if (!this.selectedUpdates.includes(parentId)) {
            this.selectedUpdates.push(parentId);
          }
        }
      } else {
        // Nếu checkbox đã được chọn thì bỏ chọn
        this.selectedUpdates = this.selectedUpdates.filter(x => x !== functionId);
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedUpdates = this.selectedUpdates.filter(el => !childFunctions.includes(el));
        }
      }
    } else if (commandId === SystemConstants.DELETE_ACTION) {
      if (checked) {
        // Nếu checkbox chưa được chọn thì thêm vào các mảng lựa chọn
        if (!this.selectedDeletes.includes(functionId)) {
          this.selectedDeletes.push(functionId);
        }
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedDeletes.push(...childFunctions);
        } else {
          if (!this.selectedDeletes.includes(parentId)) {
            this.selectedDeletes.push(parentId);
          }
        }
      } else {
        // Nếu checkbox đã được chọn thì bỏ chọn
        this.selectedDeletes = this.selectedDeletes.filter(x => x !== functionId);
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedDeletes = this.selectedDeletes.filter(el => !childFunctions.includes(el));
        }
      }
    } else if (commandId === SystemConstants.APPROVE_ACTION) {
      if (checked) {
        // Nếu checkbox chưa được chọn thì thêm vào các mảng lựa chọn
        if (!this.selectedApproves.includes(functionId)) {
          this.selectedApproves.push(functionId);
        }
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedApproves.push(...childFunctions);
        } else {
          if (!this.selectedApproves.includes(parentId)) {
            this.selectedApproves.push(parentId);
          }
        }
      } else {
        // Nếu checkbox đã được chọn thì bỏ chọn
        this.selectedApproves = this.selectedApproves.filter(x => x !== functionId);
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedApproves = this.selectedApproves.filter(el => !childFunctions.includes(el));
        }
      }
    }
  }

  clearSelections() {
    // Đặt lại tất cả các mảng lựa chọn thành rỗng
    this.selectedViews = [];
    this.selectedCreates = [];
    this.selectedUpdates = [];
    this.selectedDeletes = [];
    this.selectedApproves = [];
  }


  selectAll(checked: boolean, uniqueCode: string) {
    if (uniqueCode === SystemConstants.VIEW_ACTION) {
      if (checked) {
        // Nếu checkbox chưa được chọn, thêm tất cả các ID vào mảng lựa chọn
        this.selectedViews = this.flattenFunctions.map(x => x.id);
      } else {
        // Nếu checkbox đã được chọn, bỏ chọn tất cả các ID
        this.selectedViews = [];
      }
    } else if (uniqueCode === SystemConstants.CREATE_ACTION) {
      if (checked) {
        // Tương tự cho các checkbox khác
        this.selectedCreates = this.flattenFunctions.map(x => x.id);
      } else {
        this.selectedCreates = [];
      }
    } else if (uniqueCode === SystemConstants.UPDATE_ACTION) {
      if (checked) {
        this.selectedUpdates = this.flattenFunctions.map(x => x.id);
      } else {
        this.selectedUpdates = [];
      }
    } else if (uniqueCode === SystemConstants.DELETE_ACTION) {
      if (checked) {
        this.selectedDeletes = this.flattenFunctions.map(x => x.id);
      } else {
        this.selectedDeletes = [];
      }
    } else if (uniqueCode === SystemConstants.APPROVE_ACTION) {
      if (checked) {
        this.selectedApproves = this.flattenFunctions.map(x => x.id);
      } else {
        this.selectedApproves = [];
      }
    }
  }

  fillPermissions(roleId: any) {
    this.blockedPanel = true;
    this.subscription.add(this.rolesService.getRolePermissions(roleId)
      .subscribe((response: Permission[]) => {
        this.selectedCreates = [];
        this.selectedUpdates = [];
        this.selectedDeletes = [];
        this.selectedViews = [];
        this.selectedApproves = [];
        response.forEach(element => {
          if (element.commandId === SystemConstants.CREATE_ACTION) {
            this.selectedCreates.push(element.functionId);
          }
          if (element.commandId === SystemConstants.UPDATE_ACTION) {
            this.selectedUpdates.push(element.functionId);
          }
          if (element.commandId === SystemConstants.DELETE_ACTION) {
            this.selectedDeletes.push(element.functionId);
          }
          if (element.commandId === SystemConstants.VIEW_ACTION) {
            this.selectedViews.push(element.functionId);
          }
          if (element.commandId === SystemConstants.APPROVE_ACTION) {
            this.selectedApproves.push(element.functionId);
          }
          setTimeout(() => { this.blockedPanel = false; }, 100);
        });

      }, error => {
        setTimeout(() => { this.blockedPanel = false; }, 100);
      }));
  }

  loadAllRoles() {
    this.blockedPanel = true;
    this.subscription.add(this.rolesService.getAll()
      .subscribe((response: any) => {
        this.roles = response;
        setTimeout(() => { this.blockedPanel = false; }, 100);
      }));
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
