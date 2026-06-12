import { NgClass, NgFor, NgIf } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { ReportService } from '../../core/services/report.service';
import { ToastService } from '../../shared/components/toast.service';

const REPORTS = [
  { id: 'Employee', name: 'Employee Report', desc: 'Comprehensive list of all employees, roles, and statuses.', icon: '👥' },
  { id: 'Leave', name: 'Leave Report', desc: 'Export all leave requests with their statuses and dates.', icon: '📅' },
  { id: 'Attendance', name: 'Attendance Report', desc: 'Raw check-in and check-out logs for all employees.', icon: '⏱️' },
  { id: 'Allocation', name: 'Project Allocation', desc: 'Details of which employees are assigned to which projects.', icon: '🏢' }
];

@Component({
  standalone: true,
  imports: [NgIf, NgFor, NgClass],
  templateUrl: './report.page.html',
  styleUrls: ['./report.page.css']
})
export class ReportPageComponent {
  private readonly reportService = inject(ReportService);
  private readonly toast = inject(ToastService);

  readonly reports = REPORTS;
  readonly selectedReport = signal<string | null>(null);
  readonly downloading = signal(false);

  downloadSelected(): void {
    const rType = this.selectedReport();
    if (!rType) return;
    
    this.downloading.set(true);
    this.reportService.downloadReport(rType).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        const dateStr = new Date().toISOString().split('T')[0];
        a.download = `${rType}_Report_${dateStr}.xlsx`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
        
        this.downloading.set(false);
        this.toast.success('Download Complete', 'Your Excel report is ready.');
      },
      error: () => {
        this.downloading.set(false);
        this.toast.error('Download Failed', 'Could not generate the report. Please try again.');
      }
    });
  }
}
