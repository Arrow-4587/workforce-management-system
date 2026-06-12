// These interfaces match the backend DTOs exactly (WMS.Application.DTOs.Dashboard)

export interface AdminDashboardSummary {
  totalEmployees: number;
  totalDepartments: number;
  totalProjects: number;
  pendingLeaves: number;
  todayAttendance: number;
}

export interface ManagerDashboardSummary {
  myProjects: number;
  allocatedEmployees: number;
  pendingLeaves: number;
}

export interface EmployeeDashboardSummary {
  myProjects: number;
  myPendingLeaves: number;
  attendanceThisMonth: number;
}
