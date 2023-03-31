import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MonthlyNewKnowledgeBasesComponent } from './monthly-new-knowledge-bases.component';

describe('MonthlyNewKnowledgeBasesComponent', () => {
  let component: MonthlyNewKnowledgeBasesComponent;
  let fixture: ComponentFixture<MonthlyNewKnowledgeBasesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MonthlyNewKnowledgeBasesComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MonthlyNewKnowledgeBasesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
