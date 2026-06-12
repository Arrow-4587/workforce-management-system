// Authentication Models
export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  username: string;
  isFirstLogin: boolean;
  role: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

// Department Models
export interface DepartmentResponse {
  departmentId: number;
  departmentName: string;
  description?: string;
  createdOn: string;
  employeeCount?: number;
}

export interface CreateDepartment {
  departmentName: string;
  description?: string;
}

export interface UpdateDepartment {
  departmentId: number;
  departmentName: string;
  description?: string;
}

// Role Models
export interface RoleResponse {
  roleId: number;
  roleName: string;
  description: string;
}

export interface CreateRole {
  roleName: string;
  description: string;
}

export interface UpdateRole {
  roleId: number;
  roleName: string;
  description: string;
}

// Employee Models
export interface EmployeeResponse {
  employeeId: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  gender: string; // 'M' | 'F'
  dob: string; // ISO date string
  doj: string; // ISO date string
  departmentId: number;
  roleId: number;
  status: string; // 'Active' | 'Inactive'
  createdOn: string;
  updatedOn?: string;
  departmentName?: string;
  roleName?: string;
  department?: DepartmentResponse;
  role?: RoleResponse;
}

export interface CreateEmployee {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  gender: string;
  dob: string;
  doj: string;
  departmentId: number;
  roleId: number;
  status: string;
}

export interface UpdateEmployee {
  employeeId: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  gender: string;
  dob: string;
  doj: string;
  departmentId: number;
  roleId: number;
  status: string;
}

// Client Models
export interface ClientResponse {
  clientId: number;
  clientName: string;
  clientAddress?: string;
  clientPhoneNumber?: string;
  clientLocation?: string;
  status: boolean;
}

export interface CreateClient {
  clientName: string;
  clientAddress?: string;
  clientPhoneNumber?: string;
  clientLocation?: string;
  status: boolean;
}

export interface UpdateClient {
  clientId: number;
  clientName: string;
  clientAddress?: string;
  clientPhoneNumber?: string;
  clientLocation?: string;
  status: boolean;
}

// Project Models
export interface ProjectResponse {
  projectId: number;
  projectName: string;
  clientId: number;
  managerId: number;
  startDate: string;
  endDate?: string;
  status: string; // 'Active' | 'Completed' | etc.
  clientName?: string;
  managerName?: string;
  client?: ClientResponse;
  manager?: EmployeeResponse;
}

export interface CreateProject {
  projectName: string;
  clientId: number;
  managerId: number;
  startDate: string;
  endDate?: string;
  status: string;
}

export interface UpdateProject {
  projectId: number;
  projectName: string;
  clientId: number;
  managerId: number;
  startDate: string;
  endDate?: string;
  status: string;
}

// Allocation (EmployeeProject) Models
export interface AllocationResponse {
  allocationId: number;
  employeeId: number;
  projectId: number;
  allocatedOn: string;
  releasedOn?: string;
  employeeName?: string;
  projectName?: string;
  projectManagerName?: string;
  clientName?: string;
  employee?: EmployeeResponse;
  project?: ProjectResponse;
}

export interface AllocateEmployee {
  employeeId: number;
  projectId: number;
}

// Attendance Models
export interface AttendanceResponse {
  attendanceId: number;
  empId: number;
  checkIn: string;
  checkOut?: string;
  totalHours?: number;
  workMode: string; // 'Office' | 'Remote' | 'Hybrid'
  attendanceDate: string;
  employeeName?: string;
  employee?: EmployeeResponse;
}

export interface CheckInRequest {
  workMode: string;
}

// Leave Models
export interface LeaveResponse {
  leaveId: number;
  empId: number;
  leaveType: string;
  reason?: string;
  fromDate: string;
  toDate: string;
  status: string; // 'Pending' | 'Approved' | 'Rejected'
  appliedOn: string;
  approvedBy?: number;
  approvedOn?: string;
  employeeName?: string;
  employeeRole?: string;
  employee?: EmployeeResponse;
}

export interface ApplyLeaveRequest {
  leaveType: string;
  reason?: string;
  fromDate: string;
  toDate: string;
}

// Announcement Models
export interface AnnouncementResponse {
  announcementId: number;
  title: string;
  message: string;
  createdBy: number;
  createdOn: string;
  isActive: boolean;
  creatorName?: string;
}

export interface CreateAnnouncement {
  title: string;
  message: string;
  isActive: boolean;
}

export interface UpdateAnnouncement {
  announcementId: number;
  title: string;
  message: string;
  isActive: boolean;
}

// Audit Log Models
export interface AuditLogResponse {
  auditId: number;
  entityName: string;
  recordId: number;
  action: string; // 'Create' | 'Update' | 'Delete'
  createdBy: number;
  createdOn: string;
  username?: string;
}

// Profile Models
export interface ProfileResponse {
  employeeId: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  gender: string;
  dob: string;
  doj: string;
  departmentId: number;
  roleId: number;
  status: string;
  departmentName?: string;
  roleName?: string;
  department?: string;
  role?: string;
  username?: string;
}

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
}

// Dashboard Models
export interface AdminDashboard {
  totalEmployees: number;
  totalDepartments: number;
  totalProjects: number;
  pendingLeavesCount: number;
  todayAttendanceCount: number;
  attendanceRate: number;
  recentAuditLogs: AuditLogResponse[];
}

export interface ManagerDashboard {
  myProjectsCount: number;
  allocatedEmployeesCount: number;
  pendingLeavesCount: number;
  myProjects: ProjectResponse[];
  pendingLeaves: LeaveResponse[];
}

export interface EmployeeDashboard {
  myProjectsCount: number;
  myPendingLeavesCount: number;
  attendanceRateThisMonth: number;
  checkInStatus: {
    isCheckedIn: boolean;
    checkInTime?: string;
    workMode?: string;
  };
  myProjects: ProjectResponse[];
  myRecentLeaves: LeaveResponse[];
}
