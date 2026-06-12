import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AuditLogPageComponent } from './audit-log.page';

describe('AuditLogPageComponent', () => {
  let component: AuditLogPageComponent;
  let fixture: ComponentFixture<AuditLogPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AuditLogPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AuditLogPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
