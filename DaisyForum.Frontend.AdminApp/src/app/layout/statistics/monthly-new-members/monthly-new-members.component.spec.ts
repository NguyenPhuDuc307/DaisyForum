import { ComponentFixture, TestBed, waitForAsync as  } from '@angular/core/testing';

import { MonthlyNewMembersComponent } from './monthly-new-members.component';

describe('MonthlyNewMembersComponent', () => {
  let component: MonthlyNewMembersComponent;
  let fixture: ComponentFixture<MonthlyNewMembersComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ MonthlyNewMembersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MonthlyNewMembersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
