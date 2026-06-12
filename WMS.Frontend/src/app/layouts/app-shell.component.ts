import { NgClass, NgFor, NgIf } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../core/services/auth.service';
import { ToastComponent } from '../shared/components/toast.component';
import { ConfirmComponent } from '../shared/components/confirm.component';

interface NavItem {
  label: string;
  path: string;
  roles: string[];
  icon: string;
}

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, NgFor, NgIf, NgClass, ToastComponent, ConfirmComponent],
  templateUrl: './app-shell.component.html',
  styleUrls: ['./app-shell.component.css']
})
export class AppShellComponent {
  readonly auth = inject(AuthService);
  protected readonly sidebarCollapsed = signal(false);
  protected readonly mobileSidebarOpen = signal(false);

  private readonly NAV_ICONS: Record<string, string> = {
    dashboard: `<svg width="18" height="18" viewBox="0 0 18 18" fill="none"><rect x="1" y="1" width="7" height="7" rx="2.5" stroke="currentColor" stroke-width="1.6"/><rect x="10" y="1" width="7" height="7" rx="2.5" stroke="currentColor" stroke-width="1.6"/><rect x="1" y="10" width="7" height="7" rx="2.5" stroke="currentColor" stroke-width="1.6"/><rect x="10" y="10" width="7" height="7" rx="2.5" stroke="currentColor" stroke-width="1.6"/></svg>`,
    profile: `<svg width="18" height="18" viewBox="0 0 18 18" fill="none"><circle cx="9" cy="6" r="3.5" stroke="currentColor" stroke-width="1.6"/><path d="M2 16c0-3.314 3.134-6 7-6s7 2.686 7 6" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/></svg>`,
    attendance: `<svg width="18" height="18" viewBox="0 0 18 18" fill="none"><rect x="2" y="3" width="14" height="13" rx="2.5" stroke="currentColor" stroke-width="1.6"/><path d="M6 1v4M12 1v4M2 8h14" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/><path d="M6 12l2 2 4-3" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/></svg>`,
    leave: `<svg width="18" height="18" viewBox="0 0 18 18" fill="none"><path d="M9 2v5l3 2" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/><circle cx="9" cy="10" r="7" stroke="currentColor" stroke-width="1.6"/></svg>`,
    allocations: `<svg width="18" height="18" viewBox="0 0 18 18" fill="none"><path d="M9 3L3 7v8h4v-4h4v4h4V7L9 3z" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/></svg>`,
    departments: `<svg width="18" height="18" viewBox="0 0 18 18" fill="none"><rect x="7" y="1" width="4" height="4" rx="1.5" stroke="currentColor" stroke-width="1.6"/><rect x="1" y="13" width="4" height="4" rx="1.5" stroke="currentColor" stroke-width="1.6"/><rect x="7" y="13" width="4" height="4" rx="1.5" stroke="currentColor" stroke-width="1.6"/><rect x="13" y="13" width="4" height="4" rx="1.5" stroke="currentColor" stroke-width="1.6"/><path d="M9 5v5M3 13v-3h12v3" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/></svg>`,
    employees: `<svg width="18" height="18" viewBox="0 0 18 18" fill="none"><circle cx="7" cy="6" r="3" stroke="currentColor" stroke-width="1.6"/><path d="M1 16c0-2.761 2.686-5 6-5s6 2.239 6 5" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/><path d="M13 8c1.657 0 3 1.343 3 3v4" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/><path d="M11 4.5A3 3 0 0114 7" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/></svg>`,
    clients: `<svg width="18" height="18" viewBox="0 0 18 18" fill="none"><rect x="2" y="6" width="14" height="10" rx="2.5" stroke="currentColor" stroke-width="1.6"/><path d="M6 6V5a3 3 0 016 0v1" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/><path d="M9 11v2M7 12h4" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/></svg>`,
    projects: `<svg width="18" height="18" viewBox="0 0 18 18" fill="none"><path d="M3 4h12M3 9h8M3 14h5" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/></svg>`,
    announcements: `<svg width="18" height="18" viewBox="0 0 18 18" fill="none"><path d="M2 7l11-5v12L2 9V7z" stroke="currentColor" stroke-width="1.6" stroke-linejoin="round"/><path d="M13 9h3M5 9v5" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/></svg>`,
    roles: `<svg width="18" height="18" viewBox="0 0 18 18" fill="none"><circle cx="9" cy="9" r="7" stroke="currentColor" stroke-width="1.6"/><path d="M9 5v4l3 2" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round"/></svg>`,
    'audit-logs': `<svg width="18" height="18" viewBox="0 0 18 18" fill="none"><path d="M10 2H4a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2V8l-6-6z" stroke="currentColor" stroke-width="1.6" stroke-linejoin="round"/><path d="M10 2v6h6M6 11h6M6 14h4" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/></svg>`,
    reports: `<svg width="18" height="18" viewBox="0 0 18 18" fill="none"><path d="M12 2H6a2 2 0 00-2 2v10a2 2 0 002 2h6a2 2 0 002-2V4a2 2 0 00-2-2z" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/><path d="M7 6h4M7 10h4M7 14h2" stroke="currentColor" stroke-width="1.6" stroke-linecap="round"/></svg>`,
  };

  private readonly navItems: NavItem[] = [
    { label: 'Dashboard', path: 'dashboard', roles: ['Admin', 'Manager', 'Employee'], icon: this.NAV_ICONS['dashboard'] },
    { label: 'Profile', path: 'profile', roles: ['Admin', 'Manager', 'Employee'], icon: this.NAV_ICONS['profile'] },
    { label: 'Attendance', path: 'attendance', roles: ['Admin', 'Manager', 'Employee'], icon: this.NAV_ICONS['attendance'] },
    { label: 'Leave', path: 'leave', roles: ['Admin', 'Manager', 'Employee'], icon: this.NAV_ICONS['leave'] },
    { label: 'Allocations', path: 'allocations', roles: ['Admin', 'Manager'], icon: this.NAV_ICONS['allocations'] },
    { label: 'Departments', path: 'departments', roles: ['Admin'], icon: this.NAV_ICONS['departments'] },
    { label: 'Employees', path: 'employees', roles: ['Admin', 'Manager'], icon: this.NAV_ICONS['employees'] },
    { label: 'Clients', path: 'clients', roles: ['Admin'], icon: this.NAV_ICONS['clients'] },
    { label: 'Projects', path: 'projects', roles: ['Admin', 'Manager', 'Employee'], icon: this.NAV_ICONS['projects'] },
    { label: 'Announcements', path: 'announcements', roles: ['Admin', 'Manager', 'Employee'], icon: this.NAV_ICONS['announcements'] },
    { label: 'Roles', path: 'roles', roles: ['Admin'], icon: this.NAV_ICONS['roles'] },
    { label: 'Audit Logs', path: 'audit-logs', roles: ['Admin'], icon: this.NAV_ICONS['audit-logs'] },
    { label: 'Reports', path: 'reports', roles: ['Admin'], icon: this.NAV_ICONS['reports'] },
  ];

  isDarkMode = signal(false);

  ngOnInit() {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
      this.isDarkMode.set(true);
      document.documentElement.setAttribute('data-theme', 'dark');
    }
  }

  toggleTheme() {
    this.isDarkMode.update(v => !v);
    if (this.isDarkMode()) {
      document.documentElement.setAttribute('data-theme', 'dark');
      localStorage.setItem('theme', 'dark');
    } else {
      document.documentElement.removeAttribute('data-theme');
      localStorage.setItem('theme', 'light');
    }
  }

  readonly visibleNavItems = computed(() => {
    const role = this.auth.getUserRole();
    return this.navItems.filter(item => !role || item.roles.includes(role));
  });

  readonly pageTitle = computed(() => {
    const role = this.auth.getUserRole();
    if (role === 'Admin') return 'Admin Console';
    if (role === 'Manager') return 'Manager Workspace';
    return 'My Workspace';
  });

  readonly userInitials = computed(() =>
    (this.auth.getUsername() ?? 'WM').slice(0, 2).toUpperCase()
  );

  roleBadgeClass = computed(() => {
    const role = this.auth.getUserRole()?.toLowerCase() ?? '';
    return role;
  });

  toggleSidebar(): void { this.sidebarCollapsed.update(v => !v); }
  openMobileSidebar(): void { this.mobileSidebarOpen.set(true); }
  closeMobileSidebar(): void { this.mobileSidebarOpen.set(false); }

  logout(): void { this.auth.logout(); }
}
