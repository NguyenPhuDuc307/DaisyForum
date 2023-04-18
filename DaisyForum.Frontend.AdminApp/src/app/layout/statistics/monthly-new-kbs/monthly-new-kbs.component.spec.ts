import { ComponentFixture, TestBed, waitForAsync as  } from '@angular/core/testing';

import { MonthlyNewKbsComponent } from './monthly-new-kbs.component';

describe('MonthlyNewKbsComponent', () => {
  let component: MonthlyNewKbsComponent;
  let fixture: ComponentFixture<MonthlyNewKbsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ MonthlyNewKbsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MonthlyNewKbsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
