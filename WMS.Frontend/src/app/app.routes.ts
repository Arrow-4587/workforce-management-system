import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { AppShellComponent } from './layouts/app-shell.component';
import { LoginPageComponent } from './features/auth/login.page';
import { ChangePasswordPageComponent } from './features/auth/change-password.page';
import { DashboardPageComponent } from './features/dashboard/dashboard.page';
import { AttendancePageComponent } from './features/attendance/attendance.page';
import { LeavePageComponent } from './features/leave/leave.page';
import { DepartmentPageComponent } from './features/department/department.page';
import { EmployeePageComponent } from './features/employee/employee.page';
import { ClientPageComponent } from './features/client/client.page';
import { ProjectPageComponent } from './features/project/project.page';
import { AnnouncementPageComponent } from './features/announcement/announcement.page';
import { RolePageComponent } from './features/role/role.page';
import { AuditLogPageComponent } from './features/audit-log/audit-log.page';
import { ReportPageComponent } from './features/report/report.page';
import { AllocationPageComponent } from './features/allocation/allocation.page';
import { ProfilePageComponent } from './features/profile/profile.page';
import { ForbiddenPageComponent } from './features/system/forbidden.page';
import { UnauthorizedPageComponent } from './features/system/unauthorized.page';

export const routes: Routes = [
	{ path: 'login', component: LoginPageComponent },
	{ path: 'forbidden', component: ForbiddenPageComponent },
	{ path: 'unauthorized', component: UnauthorizedPageComponent },
	{
		path: 'change-password',
		component: ChangePasswordPageComponent,
		canActivate: [authGuard]
	},
	{
		path: '',
		component: AppShellComponent,
		canActivate: [authGuard],
		children: [
			{ path: '', pathMatch: 'full', redirectTo: 'dashboard' },
			{ path: 'dashboard', component: DashboardPageComponent },
			{ path: 'profile', component: ProfilePageComponent },
			{ path: 'attendance', component: AttendancePageComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin', 'Manager', 'Employee'] } },
			{ path: 'leave', component: LeavePageComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin', 'Manager', 'Employee'] } },
			{ path: 'allocations', component: AllocationPageComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin', 'Manager'] } },
			{ path: 'departments', component: DepartmentPageComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin'] } },
			{ path: 'employees', component: EmployeePageComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin', 'Manager'] } },
			{ path: 'clients', component: ClientPageComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin'] } },
			{ path: 'projects', component: ProjectPageComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin', 'Manager', 'Employee'] } },
			{ path: 'announcements', component: AnnouncementPageComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin', 'Manager', 'Employee'] } },
			{ path: 'roles', component: RolePageComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin'] } },
			{ path: 'audit-logs', component: AuditLogPageComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin'] } },
			{ path: 'reports', component: ReportPageComponent, canActivate: [authGuard, roleGuard], data: { roles: ['Admin'] } },
			{ path: '**', redirectTo: 'dashboard' }
		]
	},
	{ path: '**', redirectTo: 'login' }
];
