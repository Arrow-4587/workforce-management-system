# Low-Level Design (LLD) — Workforce Management System

**Document Version:** 2.0  
**Date:** June 2026  
**Author:** Indresh  
**Project:** WMS-Solution

---

## 1. Introduction

This document dives into the technical internals of the WMS system. Where the HLD explains *what* the system does and *how* the pieces connect, this document explains *how each piece is actually built* — the entity schemas, repository contracts, service logic, DTO shapes, API endpoints, Angular component architecture, and testing strategy. If you're a developer joining this project or reviewing the codebase, this is your reference.

---

## 2. Domain Layer — The Entities

The Domain layer (`WMS.Domain/Entities/`) defines 11 entity classes. These are plain C# classes with no framework dependencies — no EF Core attributes (except occasional `[Required]`/`[StringLength]` for data annotations), no NuGet references. They represent the core business objects.

### 2.1 Employee — The Central Entity

Everything in the system revolves around `Employee`. Each employee has personal details (`FirstName`, `LastName`, `Email`, `PhoneNumber`, `Gender`, `DOB`), organizational details (`DepartmentId`, `RoleId`, `DOJ`), and lifecycle metadata (`Status`, `CreatedOn`, `UpdatedOn`).

The `Status` field is a string — either `"Active"` or `"Inactive"`. When an Admin "deletes" an employee, they're typically deactivated rather than hard-deleted, preserving historical records.

Navigation properties link an Employee to their `Department`, `Role`, `UserLogin` (one-to-one), and collections of `Attendances`, `Leaves`, and `EmployeeProjects` (one-to-many).

### 2.2 UserLogin — Authentication Identity

`UserLogin` is the authentication record. It has a `Username`, a `PasswordHash` (BCrypt), a `RoleId`, and an optional `EmployeeId`. The `EmployeeId` is nullable because the seed Admin account exists without an employee record — it's a pure system account.

The one-to-one relationship between `UserLogin` and `Employee` means each employee has exactly one login. When the `EmployeeService` creates a new employee, it also creates the corresponding `UserLogin` with a default hashed password.

### 2.3 Role — Three System Personas

The `Role` entity is simple: `RoleId`, `RoleName`, and an optional `Description`. The system seeds three roles on startup: Admin (ID 1), Manager (ID 2), Employee (ID 3). Admins can create additional roles through the Role management page, though the authorization logic is currently hardcoded to recognize these three names.

### 2.4 Department — Organizational Structure

`Department` has `DepartmentId`, `DepartmentName`, `Description`, and `CreatedOn`. Three departments are seeded: IT (1), HR (2), Finance (3). Admin can create, update, or delete departments.

### 2.5 Attendance — Daily Time Tracking

Each `Attendance` record represents one day's work session. It links to an `Employee` via `EmpId` and stores `CheckIn` (timestamp), `CheckOut` (nullable timestamp), `TotalHours` (computed on checkout as `(CheckOut - CheckIn).TotalHours`), `WorkMode` ("Office" or "Remote"), and `AttendanceDate` (the date, without time).

The design allows only one open attendance record per employee per day. The service enforces this — if you try to check in when you already have an un-checked-out record, it throws an error.

### 2.6 Leave — Request Lifecycle

`Leave` tracks the full lifecycle of a leave request. Key fields: `EmpId` (who's requesting), `LeaveType` ("Sick", "Casual", "Earned"), `FromDate`/`ToDate`, `Reason` (optional), `Status` ("Pending", "Approved", "Rejected"), `AppliedOn` (auto-set), `ApprovedBy` (EmployeeId of the approver, nullable), and `ApprovedOn` (timestamp of the decision, nullable).

The `Status` field drives the entire workflow. New leaves are created as "Pending". Only pending leaves can be approved, rejected, or cancelled.

### 2.7 Project — Work Units

`Project` links to a `Client` (via `ClientId`) and a `Manager` (via `ManagerId`, which points to an Employee with the Manager role). It has a `ProjectName`, `StartDate`, optional `EndDate`, and `Status` ("Active", "Completed", "On Hold"). The navigation property `EmployeeProjects` gives you all employees allocated to this project.

### 2.8 Client — External Organizations

`Client` is straightforward: `ClientName`, `ContactEmail`, `ContactPhone`, `Address`. It has a one-to-many relationship with `Projects` — one client can have multiple projects.

### 2.9 EmployeeProject — The Allocation Join Table

This is the many-to-many bridge between `Employee` and `Project`. Each record has an `AllocationId` (PK), `EmployeeId`, `ProjectId`, `AllocationDate`, and optional `ReleaseDate`. When an Admin allocates an employee to a project, a new record is created with today's date. When they release the employee, the `ReleaseDate` is set.

### 2.10 Announcement — Company-Wide Messages

`Announcement` has a `Title` (max 200 chars, required), `Message` (max 2000 chars, required), `CreatedBy` (FK to UserLogin), `CreatedOn`, and `IsActive` (boolean, defaults to true). Announcements are displayed to all users regardless of role, but only Admin can create/edit/delete them.

### 2.11 AuditLog — The Paper Trail

`AuditLog` records system actions: `EntityName` (e.g., "Employee", "Leave"), `RecordId` (the ID of the affected record), `Action` ("Create", "Update", "Delete"), `CreatedBy` (FK to UserLogin), and `CreatedOn`. This creates a full audit trail that Admin can review.

---

## 3. Database Configuration — Entity Framework Core

### 3.1 How Relationships Are Configured

The `WmsDbContext` (in `WMS.Infrastructure/Data/`) configures all entity relationships using EF Core's Fluent API. Here's how the key relationships work:

**UserLogin → Employee (One-to-One):** Each UserLogin optionally links to an Employee via `EmployeeId`. The Employee entity has a navigation property back to `UserLogin`. EF Core handles this as a foreign key on the `UserLogins` table.

**Employee → Department (Many-to-One):** Multiple employees can belong to the same department. The foreign key `DepartmentId` on the Employees table points to Departments.

**Employee → Role (Many-to-One):** Same pattern — multiple employees can share a role. FK is `RoleId`.

**Attendance → Employee (Many-to-One):** Each attendance record belongs to one employee. FK is `EmpId`. Cascade delete — if an employee is deleted, their attendance records go too.

**Leave → Employee (Many-to-One):** Same pattern with `EmpId`.

**Project → Client (Many-to-One):** Each project belongs to one client. FK is `ClientId`.

**Project → Employee/Manager (Many-to-One):** The `ManagerId` on Project points to the Employees table, but it represents the project manager specifically. This relationship uses `.OnDelete(DeleteBehavior.Restrict)` — you can't delete an employee who manages active projects.

**EmployeeProject → Employee + Project (Many-to-One × 2):** This is the join table. Each record has FKs to both Employees and Projects.

**Announcement → UserLogin (Many-to-One):** The `CreatedBy` FK links to whoever created the announcement. Uses `.OnDelete(DeleteBehavior.Restrict)`.

**AuditLog → UserLogin (Many-to-One):** Same pattern — `CreatedBy` links to the actor. Restrict delete.

### 3.2 Seed Data — What's Pre-populated

On application startup, two things happen:

1. **EF Core migration seed data** populates the Roles table (Admin, Manager, Employee) and the Departments table (IT, HR, Finance).
2. **`DbSeeder.SeedAdminAsync()`** runs in `Program.cs` — it checks if a user with username "admin" exists. If not, it creates one with BCrypt-hashed password "Admin@123" and assigns it RoleId 1 (Admin). This means the first time you run the app against a fresh database, you immediately have a working login.

---

## 4. Repository Layer — How Data Access Is Structured

Every repository follows the same pattern: an interface in `WMS.Domain/Interfaces/` defines the contract, and a concrete class in `WMS.Infrastructure/Repositories/` implements it using `WmsDbContext`.

### 4.1 IEmployeeRepository

This is the most feature-rich repository because Employee is the central entity. It provides:

- `GetAllAsync()` — Returns all employees with their Department and Role eagerly loaded (`.Include()`).
- `GetByIdAsync(id)` — Single employee with related data.
- `SearchByNameAsync(name)` — Filters by first or last name containing the search term.
- `GetByDepartmentAsync(departmentId)` — Filters by department.
- `GetByRoleAsync(roleId)` — Filters by role.
- `GetByUserIdAsync(userId)` — Finds the employee linked to a specific UserLogin — used for "get my profile" functionality.
- `CreateAsync(employee)` — Adds and saves, returns the new ID.
- `UpdateAsync(employee)` — Marks entity as modified, saves.
- `DeleteAsync(id)` — Removes the entity.

### 4.2 IAuthRepository

Handles authentication data access:

- `GetByUsernameAsync(username)` — Finds a UserLogin by username, includes Role.
- `CreateAsync(userLogin)` — Creates a new login account.
- `UpdatePasswordAsync(userId, newPasswordHash)` — Updates the stored hash.

### 4.3 IAttendanceRepository

Focused on time tracking:

- `GetTodayAttendanceAsync(employeeId)` — Finds today's record for an employee (checks `AttendanceDate == today`). This is crucial for the check-in/check-out logic.
- `GetByEmployeeAsync(employeeId)` — Full attendance history for one employee.
- `GetMonthlyAsync(employeeId, year, month)` — Filtered by specific month.
- `CreateAsync(attendance)` — Inserts a new check-in record.
- `UpdateAsync(attendance)` — Updates for check-out (adds checkout time and total hours).

### 4.4 ILeaveRepository

Manages leave records:

- `GetByEmployeeAsync(employeeId)` — All leaves for one employee.
- `GetPendingAsync()` — All leaves with Status = "Pending" (used by Admin).
- `GetPendingForManagerAsync(managerId)` — Pending leaves for employees in the manager's projects (team-scoped filtering).
- `GetByIdAsync(leaveId)` — Single leave for approval/rejection.
- `CreateAsync(leave)` — New leave application.
- `UpdateAsync(leave)` — Status changes (approve/reject).
- `DeleteAsync(leaveId)` — Cancel (removes the record).

### 4.5 IProjectRepository

Standard CRUD plus manager filtering:

- `GetAllAsync()` — All projects with Client and Manager loaded.
- `GetByIdAsync(id)` — Single project with relations.
- `GetByManagerAsync(managerId)` — Projects managed by a specific employee.
- CRUD operations.

### 4.6 IEmployeeProjectRepository (Allocations)

Manages the employee-project join table:

- `GetByProjectAsync(projectId)` — All employees allocated to a project.
- `GetByEmployeeAsync(employeeId)` — All projects an employee is allocated to.
- `AllocateAsync(allocation)` — Creates a new allocation record.
- `ReleaseAsync(allocationId)` — Sets the release date on an existing allocation.

### 4.7 Other Repositories

- **IClientRepository** — Standard CRUD for clients.
- **IDepartmentRepository** — CRUD plus search functionality.
- **IRoleRepository** — Get all roles, get by ID, create.
- **IAnnouncementRepository** — CRUD, filtered by active status.
- **IAuditLogRepository** — Create log entries, get all logs with UserLogin info, filter by entity name.

---

## 5. Application Layer — Services and DTOs

This is where all business logic lives. Each service follows the Interface + Implementation pattern: the interface (e.g., `IEmployeeService`) defines the contract, and the implementation (e.g., `EmployeeService`) injects repository interfaces and orchestrates the business flow.

### 5.1 Auth Service — Login and Password Management

**DTOs involved:**
- `LoginRequestDto` — Incoming: `{Username, Password}`.
- `LoginResponseDto` — Outgoing: `{UserId, Username, Role, Token, EmployeeId, IsFirstLogin}`.
- `ChangePasswordDto` — Incoming: `{OldPassword, NewPassword}`.

**How `LoginAsync` works internally:**
1. Calls `IAuthRepository.GetByUsernameAsync(username)` to find the user.
2. If no user found, returns null (controller will return 401).
3. Uses `BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)` to check the password.
4. If password doesn't match, returns null.
5. Calls `IJwtService.GenerateToken(user)` to create the JWT token with claims: UserId (NameIdentifier), Username (Name), Role, EmployeeId.
6. Returns the `LoginResponseDto` with all the details the frontend needs.

**How `ChangePasswordAsync` works:**
1. Fetches the user by ID.
2. Verifies the old password with BCrypt.
3. Hashes the new password with BCrypt.
4. Calls repository to update the hash.

### 5.2 Employee Service — The Most Complex Service

**DTOs involved:**
- `CreateEmployeeDto` — All employee details: `FirstName`, `LastName`, `Email`, `PhoneNumber`, `Gender`, `DOB`, `DOJ`, `DepartmentId`, `RoleId`.
- `UpdateEmployeeDto` — Same fields plus `Status` (Active/Inactive).
- `EmployeeResponseDto` — Flattened response: `EmployeeId`, `FullName` (concatenated), `Email`, `Phone`, `DepartmentName` (resolved from navigation), `RoleName`, `Status`, `DOJ`.

**The `CreateAsync` flow — step by step:**
1. Maps `CreateEmployeeDto` to a new `Employee` entity, sets `CreatedOn = DateTime.Now`, `Status = "Active"`.
2. Calls `IEmployeeRepository.CreateAsync(employee)` — this inserts the employee into the database and returns the new `EmployeeId`.
3. Reads the `DefaultPassword` from `IConfiguration["EmployeeOnboarding:DefaultPassword"]`.
4. Hashes the default password with BCrypt.
5. Creates a new `UserLogin` with `Username = email`, `PasswordHash = hashedDefault`, `RoleId = dto.RoleId`, `EmployeeId = newEmployeeId`.
6. Calls `IAuthRepository.CreateAsync(userLogin)`.
7. Creates an `AuditLog` entry: `EntityName = "Employee"`, `RecordId = newEmployeeId`, `Action = "Create"`, `CreatedBy = actorUserId`.
8. Returns the `EmployeeResponseDto`.

This is a great example of why the service layer exists — creating an employee isn't just an INSERT. It's an INSERT + creating a login + hashing a password + audit logging. All of this is business logic that doesn't belong in the controller or the repository.

**The `GetMyProfileAsync` flow:**
1. Receives a `userId` (extracted from JWT claims by the controller).
2. Calls `IEmployeeRepository.GetByUserIdAsync(userId)` — this finds the employee linked to that UserLogin.
3. Maps to `EmployeeResponseDto` and returns.

### 5.3 Attendance Service — Time Tracking Logic

**DTOs:**
- `CheckInDto` — Just `{WorkMode}` ("Office" or "Remote").
- `AttendanceResponseDto` — `AttendanceId`, `CheckIn`, `CheckOut`, `TotalHours`, `WorkMode`, `Date`.

**CheckIn flow:**
1. Calls `IAttendanceRepository.GetTodayAttendanceAsync(employeeId)` to see if a record exists.
2. If a record exists AND `CheckOut` is null (meaning they're still checked in), throws an exception: "Already checked in today."
3. If no record or already checked out, creates a new `Attendance`: `EmpId = employeeId`, `CheckIn = DateTime.Now`, `AttendanceDate = DateTime.Today`, `WorkMode = dto.WorkMode`.
4. Saves via repository and returns the response DTO.

**CheckOut flow:**
1. Fetches today's open record (where `CheckOut` is null).
2. If no open record, throws: "No active check-in found."
3. Sets `CheckOut = DateTime.Now`.
4. Computes `TotalHours = (CheckOut - CheckIn).TotalHours`.
5. Updates via repository and returns.

### 5.4 Leave Service — Approval Workflow

**DTOs:**
- `ApplyLeaveDto` — `{LeaveType, FromDate, ToDate, Reason}`.
- `LeaveResponseDto` — `LeaveId`, `EmployeeName`, `LeaveType`, `FromDate`, `ToDate`, `Status`, `AppliedOn`, `Reason`.

**Apply flow:**
1. Creates a `Leave` entity: `EmpId = employeeId`, fields from DTO, `Status = "Pending"`, `AppliedOn = DateTime.Now`.
2. Saves and returns.

**Approve flow (`ApproveLeaveAsync`):**
1. Fetches the leave by ID.
2. Validates it's still "Pending".
3. Sets `Status = "Approved"`, `ApprovedBy = managerId`, `ApprovedOn = DateTime.Now`.
4. Updates via repository.
5. Creates an audit log entry.

**Reject flow** — Same as approve but sets `Status = "Rejected"`.

**Cancel flow (`CancelLeaveAsync`):**
1. Fetches the leave.
2. Validates that the requesting employee is the owner of the leave.
3. Validates status is still "Pending".
4. Deletes the record.

### 5.5 Dashboard Service — Aggregated Stats

The dashboard service doesn't have its own repository — it aggregates data from multiple repositories.

**Admin Dashboard** returns:
- `TotalEmployees` — count from employee repository (active only).
- `ActiveProjects` — count from project repository (Active status).
- `PendingLeaves` — count from leave repository (Pending status).
- `PresentToday` — count of employees with today's attendance and no checkout.
- `RecentLeaveRequests` — last N pending leaves.
- `RecentEmployees` — last N created employees.

**Manager Dashboard** returns:
- `TeamSize` — count of employees allocated to the manager's projects.
- `MyProjects` — count of projects managed by this manager.
- `PendingLeavesInTeam` — pending leaves from team members.
- `PresentTodayInTeam` — who's checked in today from the team.

**Employee Dashboard** returns:
- `TodayAttendance` — today's check-in/out status.
- `LeaveBalance` — remaining leaves (if tracked).
- `MyProjects` — list of allocated projects.
- `Announcements` — active announcements.

### 5.6 Report Service — Excel Generation

```csharp
Task<byte[]> GenerateExcelReportAsync(string reportType);
```

The report type parameter accepts: `"employee"`, `"attendance"`, `"leave"`, `"project"`. The service uses EPPlus to generate an in-memory `.xlsx` file:

1. Fetches the relevant data from repositories.
2. Creates an ExcelPackage.
3. Adds headers and rows.
4. Returns the byte array.

The controller sets `Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` and returns the bytes as a file download.

### 5.7 JWT Service — Token Generation

Located in `WMS.Infrastructure/Services/JWT/` (because it depends on external crypto libraries), the JWT service reads configuration values and generates tokens:

**Claims embedded in every token:**
- `ClaimTypes.NameIdentifier` → UserId (used by controllers to identify the actor).
- `ClaimTypes.Name` → Username (for display).
- `ClaimTypes.Role` → RoleName (used by `[Authorize(Roles)]` attribute).
- `"EmployeeId"` → The linked EmployeeId (null for Admin).

**Configuration values used:**
- `Jwt:Key` — HMAC-SHA256 signing key (minimum 32 characters).
- `Jwt:Issuer` — "WMSAPI".
- `Jwt:Audience` — "WMSClient".

The token validation parameters in `Program.cs` mirror these: it validates the issuer, audience, lifetime, and signing key on every incoming request.

### 5.8 Other Services

- **AllocationService** — Allocates employees to projects (creates EmployeeProject records), releases them (sets release date), fetches allocations filtered by project or employee.
- **AnnouncementService** — CRUD operations on announcements with audit logging.
- **AuditLogService** — Fetches audit logs for Admin review, creates new entries (called by other services).
- **ClientService** — Standard CRUD for clients with audit logging on mutations.
- **DepartmentService** — CRUD for departments, includes search capability.
- **ProjectService** — CRUD for projects, includes manager-scoped filtering.
- **RoleService** — Lists all roles, allows Admin to create new ones.
- **ProfileService** — Fetches and updates the logged-in user's own profile data.

---

## 6. API Endpoint Specification

### 6.1 Auth Controller — `/api/auth`

**`POST /login`** — Anonymous (no token needed). Accepts `{username, password}`, returns `{token, role, userId, employeeId, isFirstLogin}`. This is the entry point to the entire system.

**`GET /profile`** — Any authenticated user. Returns the current user's basic info from the JWT claims.

**`POST /change-password`** — Any authenticated user. Accepts `{oldPassword, newPassword}`. Validates the old password before accepting the new one.

### 6.2 Employee Controller — `/api/employee`

**`GET /`** — Admin, Manager. Returns all employees with department and role names resolved.

**`GET /{id}`** — Admin, Manager. Single employee by ID.

**`POST /`** — Admin only. Creates employee + auto-generates login. Body: `CreateEmployeeDto`.

**`PUT /{id}`** — Admin only. Updates employee details. Body: `UpdateEmployeeDto`.

**`DELETE /{id}`** — Admin only. Deletes or deactivates the employee.

**`GET /search?name=`** — Admin, Manager. Search by name substring.

**`GET /department/{departmentId}`** — Admin, Manager. Filter by department.

**`GET /role/{roleId}`** — Admin only. Filter by role.

**`GET /me`** — Any authenticated user. Returns the employee record linked to the current JWT token.

### 6.3 Attendance Controller — `/api/attendance`

**`POST /checkin`** — Any authenticated user. Body: `{workMode}`. Employee ID is extracted from the JWT — you don't pass it.

**`POST /checkout`** — Any authenticated user. No body needed — finds today's open record for the calling employee.

**`GET /my`** — Any authenticated user. Returns the calling employee's full attendance history.

**`GET /employee/{id}`** — Admin, Manager. View another employee's attendance.

**`GET /monthly?year=&month=`** — Any authenticated user. Monthly view of attendance.

### 6.4 Leave Controller — `/api/leave`

**`POST /apply`** — Employee, Manager (not Admin). Body: `{leaveType, fromDate, toDate, reason}`. Creates a pending leave.

**`DELETE /cancel/{id}`** — The leave owner. Cancels a pending leave.

**`GET /my`** — Any authenticated user. Returns the calling employee's leave history.

**`GET /pending`** — Admin (all pending), Manager (team pending). Returns leaves awaiting action.

**`POST /approve/{id}`** — Admin, Manager. Approves a pending leave.

**`POST /reject/{id}`** — Admin, Manager. Rejects a pending leave.

### 6.5 Project Controller — `/api/project`

**`GET /`** — Admin, Manager. All projects with client and manager info.

**`GET /{id}`** — Admin, Manager. Single project details.

**`POST /`** — Admin only. Creates a project linked to a client and manager.

**`PUT /{id}`** — Admin only. Updates project details.

**`DELETE /{id}`** — Admin only. Deletes a project.

**`GET /my-projects`** — Manager. Projects where the calling user is the assigned manager.

### 6.6 Allocation Controller — `/api/allocation`

**`POST /`** — Admin only. Allocates an employee to a project. Body: `{employeeId, projectId}`.

**`DELETE /{id}`** — Admin only. Releases an employee from a project.

**`GET /project/{id}`** — Admin, Manager. All allocations for a specific project.

**`GET /employee/{id}`** — Admin, Manager. All allocations for a specific employee.

**`GET /my-projects`** — Employee. The calling employee's own project allocations.

### 6.7 Dashboard Controller — `/api/dashboard`

**`GET /admin`** — Admin only. Returns org-wide aggregate stats.

**`GET /manager`** — Manager only. Returns team-level stats.

**`GET /employee`** — Employee only. Returns personal stats and info.

### 6.8 Other Controllers

**Department Controller** (`/api/department`) — Full CRUD (Admin only), search.

**Client Controller** (`/api/client`) — Full CRUD (Admin only).

**Announcement Controller** (`/api/announcement`) — Create/Update/Delete (Admin), Read (all roles).

**Role Controller** (`/api/role`) — List all and create new (Admin only).

**AuditLog Controller** (`/api/auditlog`) — Read audit trail (Admin only), filter by entity name.

**Report Controller** (`/api/report`) — `GET /export/{reportType}` (Admin only). Returns .xlsx file download. Report types: employee, attendance, leave, project.

**Profile Controller** (`/api/profile`) — Get and update own profile (any authenticated user), change password.

---

## 7. Infrastructure Layer — Implementation Details

### 7.1 The Repository Pattern in Practice

Every concrete repository in `WMS.Infrastructure/Repositories/` follows this pattern:

1. The class receives `WmsDbContext` through constructor injection.
2. Read methods use LINQ queries with `.Include()` for eager loading of navigation properties. For example, fetching employees also loads their Department and Role in a single query.
3. Write methods add/update entities through the DbContext's `DbSet<T>` and call `SaveChangesAsync()`.
4. All methods are async (return `Task<T>`) to avoid blocking threads during database I/O.

There's also a Generic Repository base in `WMS.Infrastructure/Repositories/Generic/` that provides basic CRUD operations. Domain-specific repositories extend or compose this base to add specialized query methods.

### 7.2 WmsDbContext

The `WmsDbContext` class:
- Defines `DbSet<T>` properties for all 11 entities.
- Overrides `OnModelCreating` to configure relationships, cascade behaviors, and seed data.
- Uses the connection string from `appsettings.json` (`ConnectionStrings:DefaultConnection`).

### 7.3 Database Migrations

EF Core migrations are stored in `WMS.Infrastructure/Migrations/`. The initial migration creates all 11 tables with their relationships and indexes. To apply migrations: `dotnet ef database update` from the WMS.API project directory.

---

## 8. Frontend Architecture — Deep Dive

### 8.1 How a Feature Page Works (Employee Example)

Let's trace through the Employee Management page to see how frontend components interact with services and the backend:

**The Component (`features/employee/employee.page.ts`):**
This is a standalone Angular component. It injects `EmployeeService` and uses Angular Signals for state management. When the page loads, it calls `employeeService.getAll()` and subscribes to the response. The data is stored in a local signal, and the template renders it in a data table.

When Admin clicks "Add Employee", a form appears. On submit, the component calls `employeeService.create(formData)`, which returns an Observable. On success, it refreshes the employee list and shows a success notification.

**The Service (`core/services/employee.service.ts`):**
`EmployeeService` constructs URLs using `environment.apiUrl` + `ApiEndpoints.Employee.Base`. For example, `getAll()` sends `GET {apiUrl}/employee`. The service doesn't handle authentication — the `authInterceptor` automatically attaches the JWT token.

**The Interceptor (`core/interceptors/auth.interceptor.ts`):**
Before the request leaves Angular, the interceptor reads `AuthService.currentUser()`. If a user is logged in, it clones the request and adds `Authorization: Bearer <token>`. If the response comes back as 401 (and it's not a login request), the interceptor logs the user out and redirects to login.

**The API Constants (`core/constants/api.constants.ts`):**
All endpoint paths are centralized here. `ApiEndpoints.Employee.Base` is `/employee`, `ApiEndpoints.Employee.ById(id)` returns `/employee/${id}`, etc. This single-source-of-truth approach means changing a URL path only requires a change in one file.

**The Models (`core/models/wms.models.ts`):**
TypeScript interfaces like `EmployeeResponse`, `CreateEmployee`, `UpdateEmployee` mirror the backend DTOs. This gives you compile-time type checking — if the backend changes a field name, TypeScript catches the mismatch during build.

### 8.2 How the App Shell Component Works

`AppShellComponent` is the authenticated layout wrapper. It renders:
- A **sidebar** on the left with navigation links. The sidebar dynamically shows/hides menu items based on the user's role from `AuthService.getUserRole()`. For example, "Audit Logs" and "Reports" only appear for Admin.
- A **header** at the top with the user's name, role badge, and a logout button.
- A **`<router-outlet>`** in the main content area where feature pages render.

### 8.3 How Guards Protect Routes

**authGuard** runs before any authenticated route activates:
1. Checks `authService.isAuthenticated()` — which reads the `currentUser` signal.
2. If the user has a session but `isFirstLogin` is true, redirects to `/change-password`.
3. If no session exists, redirects to `/login` with a `returnUrl` query parameter.

**roleGuard** runs after `authGuard` on role-restricted routes:
1. Reads `route.data['roles']` — the array of allowed roles defined in the route config.
2. Reads the user's actual role from `authService.getUserRole()`.
3. If the user's role is in the allowed list, grants access. Otherwise, redirects to `/forbidden`.

### 8.4 Design System — CSS Custom Properties

The global stylesheet (`styles.scss`) defines a design system using CSS custom properties:

**Color tokens:** `--primary` (vibrant blue), `--success` (emerald green), `--warning` (amber), `--danger` (red), `--text` (dark slate), `--muted` (slate gray), `--surface` (white card background), `--bg` (light gray page background).

**Dark mode:** The `[data-theme='dark']` selector overrides these variables — `--bg` becomes dark gray, `--surface` becomes darker gray, `--text` becomes light. Toggling the `data-theme` attribute on the `<html>` element switches the entire UI.

**Layout tokens:** `--sidebar-width: 272px` controls the sidebar width globally. `--transition: 0.3s cubic-bezier(0.4, 0, 0.2, 1)` provides smooth animations across all interactive elements.

---

## 9. How the CI/CD Pipeline Deploys Changes

The pipeline in `WMS.DevOps/.azure-pipelines.yml` automates the backend deployment in two stages:

**Stage 1 — BuildBackend:**
1. A `windows-latest` VM agent picks up the job.
2. Installs .NET 8 SDK using `UseDotNet@2`.
3. Restores NuGet packages for `WMS.API.csproj` (which transitively restores all project dependencies).
4. Builds in Release configuration.
5. Publishes to `$(Build.ArtifactStagingDirectory)/api`.
6. Publishes the output as a build artifact named `api`.

**Stage 2 — DeployBackend:**
1. Downloads the `api` artifact from Stage 1.
2. Uses `AzureRmWebAppDeployment@4` with MSDeploy to push the published output to Azure App Service `wms-api-indresh-2026`.
3. The Azure Service Connection (`Azure-ServiceConnection`) provides the credentials.

The test step is currently commented out (to be enabled once the test suite is expanded). When enabled, it would run `dotnet test` on all `*Tests*.csproj` projects between build and publish.

---

## 10. Error Handling Strategy

The system handles errors at multiple levels:

**Invalid login credentials** — The auth service returns null, and the controller sends 401 Unauthorized with "Invalid username or password."

**Missing or expired JWT token** — ASP.NET Core's JWT middleware returns 401 automatically. The Angular interceptor catches this and redirects to login.

**Insufficient role** — The `[Authorize(Roles)]` middleware returns 403 Forbidden. On the frontend, the roleGuard prevents navigation before the request is even made.

**Resource not found** — Controllers check for null results and return 404. For example, `GetById` returns `NotFound()` if the employee doesn't exist.

**Business rule violations** — Services throw exceptions for things like "Already checked in today" or "Leave is not in Pending status." Controllers catch these and return 400 Bad Request with the error message.

**Admin trying to apply leave** — The leave service explicitly blocks Admin role from applying leave with a 403 response.

**Report generation failures** — Caught and returned as 500 Internal Server Error with a generic message (to avoid leaking internal details).

---

## 11. Testing Strategy

### Backend Tests (WMS.Tests/Services/)

The test project is organized by feature, matching the service structure:

**Auth tests (Services/Auth/):**
- Verify that valid credentials return a token.
- Verify that invalid credentials return null.
- Verify that BCrypt hashing and verification work correctly.
- Verify that the JWT token contains the expected claims.

**Attendance tests (Services/Attendance/):**
- Verify that check-in creates a new record with correct timestamps.
- Verify that checking in twice on the same day throws an error.
- Verify that check-out calculates total hours correctly.
- Verify that check-out without an open check-in throws an error.

**Leave tests (Services/Leave/):**
- Verify that applying leave creates a Pending record.
- Verify that approving a leave changes status and sets approver info.
- Verify that rejecting works the same way with "Rejected" status.
- Verify that cancelling is only allowed for pending leaves owned by the requester.

**Dashboard tests (Services/Dashboard/):**
- Verify that admin dashboard returns correct aggregate counts.
- Verify that manager dashboard scopes to team members.
- Verify that employee dashboard returns personal data only.

Tests use mocked repository interfaces — the actual database is never involved. This makes tests fast, deterministic, and independent of database state.

### Frontend Tests

Angular components and services are tested with Vitest. Service tests mock the `HttpClient` and verify that the correct URLs are called with the right HTTP methods and request bodies. Component tests verify rendering logic — for example, that the sidebar shows "Audit Logs" only when the user's role is Admin.

---

## 12. Screenshots

### Employee Management Page
![Employee Management](images/wms_employees.png)

### Attendance Tracking Page
![Attendance](images/wms_attendance.png)

### Leave Management Page
![Leave Management](images/wms_leave.png)

---

*Document prepared as part of WMS Solution documentation suite.*
