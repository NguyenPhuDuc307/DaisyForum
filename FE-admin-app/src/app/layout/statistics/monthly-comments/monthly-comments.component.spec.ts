import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MonthlyCommentsComponent } from './monthly-comments.component';

describe('MonthlyCommentsComponent', () => {
  let component: MonthlyCommentsComponent;
  let fixture: ComponentFixture<MonthlyCommentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MonthlyCommentsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MonthlyCommentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
