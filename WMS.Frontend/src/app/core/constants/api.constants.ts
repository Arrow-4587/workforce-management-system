export const ApiEndpoints = {
  Auth: {
    Login: '/auth/login',
    Profile: '/auth/profile',
    ChangePassword: '/auth/change-password'
  },
  Department: {
    Base: '/department',
    Search: '/department/search',
    ById: (id: number | string) => `/department/${id}`
  },
  Employee: {
    Base: '/employee',
    Search: '/employee/search',
    Me: '/employee/me',
    ById: (id: number | string) => `/employee/${id}`,
    ByDepartment: (id: number | string) => `/employee/department/${id}`,
    ByRole: (id: number | string) => `/employee/role/${id}`
  },
  Attendance: {
    CheckIn: '/attendance/checkin',
    CheckOut: '/attendance/checkout',
    My: '/attendance/my',
    ByEmployee: (id: number | string) => `/attendance/employee/${id}`,
    Monthly: '/attendance/monthly'
  },
  Leave: {
    Apply: '/leave/apply',
    Cancel: (id: number | string) => `/leave/cancel/${id}`,
    My: '/leave/my',
    Pending: '/leave/pending',
    Approve: (id: number | string) => `/leave/approve/${id}`,
    Reject: (id: number | string) => `/leave/reject/${id}`
  },
  Client: {
    Base: '/client',
    ById: (id: number | string) => `/client/${id}`
  },
  Project: {
    Base: '/project',
    ById: (id: number | string) => `/project/${id}`,
    MyProjects: '/project/my-projects'
  },
  Allocation: {
    Base: '/allocation',
    ById: (id: number | string) => `/allocation/${id}`,
    ByProject: (id: number | string) => `/allocation/project/${id}`,
    ByEmployee: (id: number | string) => `/allocation/employee/${id}`,
    MyProjects: '/allocation/my-projects'
  },
  Dashboard: {
    Admin: '/dashboard/admin',
    Manager: '/dashboard/manager',
    Employee: '/dashboard/employee'
  },
  Announcement: {
    Base: '/announcement',
    ById: (id: number | string) => `/announcement/${id}`
  },
  Role: {
    Base: '/role',
    ById: (id: number | string) => `/role/${id}`
  },
  AuditLog: {
    Base: '/auditlog',
    ByEntity: (name: string) => `/auditlog/entity/${name}`
  },
  Profile: {
    Base: '/profile',
    ChangePassword: '/profile/change-password'
  }
};
