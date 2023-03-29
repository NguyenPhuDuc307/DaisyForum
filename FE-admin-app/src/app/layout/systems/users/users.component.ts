import { Component, OnInit } from "@angular/core";
import { Observable } from "rxjs";
import { User } from "@app/shared/models";
import { UsersService } from "@app/shared/services/users.service";

@Component({
  selector: "app-users",
  templateUrl: "./users.component.html",
  styleUrls: ["./users.component.scss"],
})
export class UsersComponent implements OnInit {
  public users$: Observable<User[]>;

  constructor(private usersService: UsersService) {}

  ngOnInit() {
    this.users$ = this.usersService.getAll();
  }
}
