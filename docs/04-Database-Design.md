# 4. Database Design

Database: **Microsoft SQL Server** (EF Core Code-First). Naming: table name = DbSet name (plural). All tables use an `int IDENTITY` primary key.

## Table overview

| # | Table | Purpose |
|---|---|---|
| 1 | Roles | System roles (Admin, HRManager, Recruiter, Employee, Candidate). |
| 2 | Users | Login account for every person in the system. |
| 3 | Departments | Organisation departments. |
| 4 | Designations | Job titles / grades. |
| 5 | Employees | Employee profile linked to a user account. |
| 6 | JobOpenings | Vacancy published by HR / recruiters. |
| 7 | Candidates | Job seeker profile (self-registered or added by HR). |
| 8 | JobApplications | A candidate's application for a job opening. |
| 9 | Interviews | Interview round scheduled for an application. |
| 10 | OfferLetters | Job offer issued to a candidate (max one per application). |
| 11 | LeaveTypes | Leave categories and yearly entitlement. |
| 12 | LeaveRequests | Employee leave application and its approval. |
| 13 | Attendances | Daily attendance record per employee. |
| 14 | Payrolls | Monthly salary record per employee. |
| 15 | PerformanceReviews | Periodic performance review of an employee. |

## Roles

System roles (Admin, HRManager, Recruiter, Employee, Candidate).

| Column | SQL type | Null | Key / constraint | Notes |
|---|---|---|---|---|
| RoleId | int | NOT NULL | PK |  |
| RoleName | nvarchar(50) | NOT NULL | UNIQUE | default string.Empty |

## Users

Login account for every person in the system.

| Column | SQL type | Null | Key / constraint | Notes |
|---|---|---|---|---|
| UserId | int | NOT NULL | PK |  |
| FullName | nvarchar(100) | NOT NULL |  | default string.Empty |
| Email | nvarchar(150) | NOT NULL | UNIQUE | default string.Empty |
| PasswordHash | nvarchar(256) | NOT NULL |  | PBKDF2 hash, never plain text; default string.Empty |
| PhoneNumber | nvarchar(20) | NULL |  |  |
| RoleId | int | NOT NULL | FK -> Roles |  |
| IsActive | bit | NOT NULL |  | default true |
| CreatedAt | datetime2 | NOT NULL |  | default UTC now (set by app) |

## Departments

Organisation departments.

| Column | SQL type | Null | Key / constraint | Notes |
|---|---|---|---|---|
| DepartmentId | int | NOT NULL | PK |  |
| Name | nvarchar(100) | NOT NULL | UNIQUE | default string.Empty |
| Description | nvarchar(500) | NULL |  |  |
| IsActive | bit | NOT NULL |  | default true |

## Designations

Job titles / grades.

| Column | SQL type | Null | Key / constraint | Notes |
|---|---|---|---|---|
| DesignationId | int | NOT NULL | PK |  |
| Title | nvarchar(100) | NOT NULL | UNIQUE | default string.Empty |
| BaseSalary | decimal(18,2) | NOT NULL |  |  |

## Employees

Employee profile linked to a user account.

| Column | SQL type | Null | Key / constraint | Notes |
|---|---|---|---|---|
| EmployeeId | int | NOT NULL | PK |  |
| EmployeeCode | nvarchar(20) | NOT NULL | UNIQUE, e.g. EMP00001 | default string.Empty |
| UserId | int | NOT NULL | FK -> Users, UNIQUE (1:1) |  |
| DepartmentId | int | NOT NULL | FK -> Departments |  |
| DesignationId | int | NOT NULL | FK -> Designations |  |
| ManagerId | int | NULL | FK -> Employees (self) |  |
| JoinDate | date | NOT NULL |  |  |
| Salary | decimal(18,2) | NOT NULL |  |  |
| Status | nvarchar(20) | NOT NULL |  | Active / OnLeave / Terminated; default EmployeeStatus.Active |
| Address | nvarchar(250) | NULL |  |  |

## JobOpenings

Vacancy published by HR / recruiters.

| Column | SQL type | Null | Key / constraint | Notes |
|---|---|---|---|---|
| JobOpeningId | int | NOT NULL | PK |  |
| Title | nvarchar(150) | NOT NULL |  | default string.Empty |
| DepartmentId | int | NOT NULL | FK -> Departments |  |
| DesignationId | int | NOT NULL | FK -> Designations |  |
| Vacancies | int | NOT NULL |  | default 1 |
| Description | nvarchar(max) | NOT NULL |  | default string.Empty |
| Requirements | nvarchar(max) | NULL |  |  |
| SalaryMin | decimal(18,2) | NOT NULL |  |  |
| SalaryMax | decimal(18,2) | NOT NULL |  |  |
| EmploymentType | nvarchar(30) | NOT NULL |  | default "Full-time" |
| Deadline | date | NOT NULL |  |  |
| Status | nvarchar(20) | NOT NULL |  | Draft / Open / Closed; default JobStatus.Draft |
| PostedById | int | NOT NULL | FK -> Users |  |
| CreatedAt | datetime2 | NOT NULL |  | default UTC now (set by app) |

## Candidates

Job seeker profile (self-registered or added by HR).

| Column | SQL type | Null | Key / constraint | Notes |
|---|---|---|---|---|
| CandidateId | int | NOT NULL | PK |  |
| UserId | int | NULL | FK -> Users (optional, 1:1) |  |
| FullName | nvarchar(100) | NOT NULL |  | default string.Empty |
| Email | nvarchar(150) | NOT NULL | UNIQUE | default string.Empty |
| Phone | nvarchar(20) | NULL |  |  |
| Skills | nvarchar(500) | NULL |  |  |
| ExperienceYears | int | NOT NULL |  |  |
| ResumePath | nvarchar(300) | NULL |  |  |
| CreatedAt | datetime2 | NOT NULL |  | default UTC now (set by app) |

## JobApplications

A candidate's application for a job opening.

| Column | SQL type | Null | Key / constraint | Notes |
|---|---|---|---|---|
| ApplicationId | int | NOT NULL | PK |  |
| JobOpeningId | int | NOT NULL | FK -> JobOpenings |  |
| CandidateId | int | NOT NULL | FK -> Candidates; UNIQUE with JobOpeningId |  |
| AppliedAt | datetime2 | NOT NULL |  | default UTC now (set by app) |
| Status | nvarchar(20) | NOT NULL |  | Applied/Shortlisted/Interview/Offered/Hired/Rejected; default ApplicationStatus.Applied |
| CoverLetter | nvarchar(2000) | NULL |  |  |
| Notes | nvarchar(1000) | NULL |  |  |

## Interviews

Interview round scheduled for an application.

| Column | SQL type | Null | Key / constraint | Notes |
|---|---|---|---|---|
| InterviewId | int | NOT NULL | PK |  |
| ApplicationId | int | NOT NULL | FK -> JobApplications |  |
| InterviewerId | int | NOT NULL | FK -> Employees |  |
| ScheduledAt | datetime2 | NOT NULL |  |  |
| Mode | nvarchar(30) | NOT NULL |  | Onsite / Online / Phone; default "Onsite" |
| Location | nvarchar(150) | NULL |  |  |
| Status | nvarchar(20) | NOT NULL |  | Scheduled/Completed/Cancelled; default InterviewStatus.Scheduled |
| Score | int | NULL |  | 1-10 |
| Feedback | nvarchar(1000) | NULL |  |  |

## OfferLetters

Job offer issued to a candidate (max one per application).

| Column | SQL type | Null | Key / constraint | Notes |
|---|---|---|---|---|
| OfferId | int | NOT NULL | PK |  |
| ApplicationId | int | NOT NULL | FK -> JobApplications, UNIQUE (1:1) |  |
| OfferedSalary | decimal(18,2) | NOT NULL |  |  |
| JoiningDate | date | NOT NULL |  |  |
| Status | nvarchar(20) | NOT NULL |  | Issued/Accepted/Declined; default OfferStatus.Issued |
| IssuedAt | datetime2 | NOT NULL |  | default UTC now (set by app) |

## LeaveTypes

Leave categories and yearly entitlement.

| Column | SQL type | Null | Key / constraint | Notes |
|---|---|---|---|---|
| LeaveTypeId | int | NOT NULL | PK |  |
| Name | nvarchar(50) | NOT NULL | UNIQUE | default string.Empty |
| DaysPerYear | int | NOT NULL |  |  |

## LeaveRequests

Employee leave application and its approval.

| Column | SQL type | Null | Key / constraint | Notes |
|---|---|---|---|---|
| LeaveRequestId | int | NOT NULL | PK |  |
| EmployeeId | int | NOT NULL | FK -> Employees |  |
| LeaveTypeId | int | NOT NULL | FK -> LeaveTypes |  |
| FromDate | date | NOT NULL |  |  |
| ToDate | date | NOT NULL |  |  |
| TotalDays | int | NOT NULL |  | calculated |
| Reason | nvarchar(500) | NULL |  |  |
| Status | nvarchar(20) | NOT NULL |  | Pending/Approved/Rejected/Cancelled; default LeaveStatus.Pending |
| ApprovedById | int | NULL | FK -> Employees |  |
| AppliedAt | datetime2 | NOT NULL |  | default UTC now (set by app) |

## Attendances

Daily attendance record per employee.

| Column | SQL type | Null | Key / constraint | Notes |
|---|---|---|---|---|
| AttendanceId | int | NOT NULL | PK |  |
| EmployeeId | int | NOT NULL | FK -> Employees; UNIQUE with Date |  |
| Date | date | NOT NULL |  |  |
| CheckIn | time | NULL |  |  |
| CheckOut | time | NULL |  |  |
| Status | nvarchar(20) | NOT NULL |  | Present/Absent/Late/OnLeave; default AttendanceStatus.Present |

## Payrolls

Monthly salary record per employee.

| Column | SQL type | Null | Key / constraint | Notes |
|---|---|---|---|---|
| PayrollId | int | NOT NULL | PK |  |
| EmployeeId | int | NOT NULL | FK -> Employees |  |
| Month | int | NOT NULL |  | 1-12; UNIQUE with EmployeeId+Year |
| Year | int | NOT NULL |  |  |
| BasicSalary | decimal(18,2) | NOT NULL |  |  |
| Allowance | decimal(18,2) | NOT NULL |  |  |
| Deduction | decimal(18,2) | NOT NULL |  |  |
| NetSalary | decimal(18,2) | NOT NULL |  | Basic + Allowance - Deduction |
| Status | nvarchar(20) | NOT NULL |  | Generated/Paid; default PayrollStatus.Generated |
| GeneratedAt | datetime2 | NOT NULL |  | default UTC now (set by app) |

## PerformanceReviews

Periodic performance review of an employee.

| Column | SQL type | Null | Key / constraint | Notes |
|---|---|---|---|---|
| ReviewId | int | NOT NULL | PK |  |
| EmployeeId | int | NOT NULL | FK -> Employees (reviewed) |  |
| ReviewerId | int | NOT NULL | FK -> Employees (reviewer) |  |
| Period | nvarchar(30) | NOT NULL |  | e.g. 2026-H1; default string.Empty |
| Rating | int | NOT NULL |  | 1-5 |
| Comments | nvarchar(1000) | NULL |  |  |
| ReviewedAt | datetime2 | NOT NULL |  | default UTC now (set by app) |

## Composite unique indexes

| Table | Columns | Rule |
|---|---|---|
| JobApplications | JobOpeningId, CandidateId | A candidate can apply to a job only once |
| Attendances | EmployeeId, Date | One attendance row per employee per day |
| Payrolls | EmployeeId, Month, Year | One payroll row per employee per month |

## Seed data (inserted by the first migration)

| Table | Rows |
|---|---|
| Roles | 1 Admin, 2 HRManager, 3 Recruiter, 4 Employee, 5 Candidate |
| LeaveTypes | Annual Leave (20), Sick Leave (10), Casual Leave (10) |
| Users | First Admin account is created at application start from `SeedAdmin` in `appsettings.json` |

## Referential integrity

Every foreign key uses `ON DELETE NO ACTION` (Restrict). This avoids SQL Server *multiple cascade paths* errors and prevents HR data from disappearing silently. When a user tries to delete a record that is still referenced, the Facade catches the database error and shows *"This record is used by other data and cannot be deleted."*

## Normalisation

- **1NF** - all columns are atomic; repeating groups (applications, interviews, leaves) are separate tables.
- **2NF** - every non-key column depends on the whole key; composite uniqueness is only used as an index, the PK is a surrogate `int`.
- **3NF** - lookups (Role, Department, Designation, LeaveType) are separate tables; no transitive dependencies. `LeaveRequests.TotalDays` and `Payrolls.NetSalary` are intentionally stored calculated values (historical snapshot: later changes to leave dates or salary must not rewrite old records).
