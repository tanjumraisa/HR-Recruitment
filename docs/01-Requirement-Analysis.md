# 1. Requirement Analysis

**Project:** HR and Recruitment  |  **Stack:** ASP.NET Core MVC, C#, EF Core, SQL Server  |  **Architecture:** same layered structure as CRM-Education

> The feature list below is a complete proposal built from a typical HR + recruitment workflow. Remove or add rows here and in the matching entity/facade if your teacher's requirement differs.

## 1.1 Problem statement

Small and medium organisations manage job vacancies, applicants, interviews, employee records, attendance, leave and salary in spreadsheets and paper. This causes duplicated data, lost applications, wrong leave balances and slow payroll. The system gives one web application for the full cycle: **publish job -> receive applications -> interview -> offer -> hire -> manage the employee**.

## 1.2 Scope

In scope: recruitment, employee records, attendance, leave, payroll and performance review. Out of scope (future work): resume file upload, e-mail / SMS notification, online payment gateway, biometric attendance, tax slabs, multi-company support.

## 1.3 Functional requirements ("I want this feature")

### Authentication and accounts

- FR-01 Candidate can register with name, email, phone and password (password >= 8 characters).
- FR-02 Users log in with email and password; passwords are stored as salted hashes.
- FR-03 Role-based access: Admin, HRManager, Recruiter, Employee, Candidate.
- FR-04 Admin can create staff users, change a user's role, deactivate or delete users.

### Recruitment

- FR-05 HR/Recruiter can create, edit, publish, close and delete job openings with department, designation, vacancies, salary range and deadline.
- FR-06 Public Careers page lists open jobs with search; visitors can read job details.
- FR-07 Candidates can apply once per job with a cover letter and see their application status.
- FR-08 HR/Recruiter can maintain candidate profiles (skills, experience, resume link).
- FR-09 HR/Recruiter can shortlist or reject applications; the hiring pipeline is Applied -> Shortlisted -> Interview -> Offered -> Hired (or Rejected).
- FR-10 HR/Recruiter can schedule interviews (interviewer, time, mode, location) and record score and feedback.
- FR-11 HR Manager can issue an offer letter (salary, joining date) and record Accepted / Declined.
- FR-12 When an offer is accepted the candidate becomes an Employee automatically (profile created, role changed).

### Core HR

- FR-13 Manage departments and designations.
- FR-14 Manage employees: code, department, designation, reporting manager, join date, salary, status.
- FR-15 Employees check in / check out; HR can correct attendance. Late arrival is flagged automatically.
- FR-16 Manage leave types with yearly entitlement; employees apply for leave; HR approves or rejects.
- FR-17 Generate monthly payroll for all active employees and mark it paid; HR can also edit single payroll rows.
- FR-18 HR can record performance reviews (period, rating 1-5, comments).

### Reporting

- FR-19 Dashboard with active employees, open jobs, applications, pending leaves, hiring pipeline and recent applications.
- FR-20 Employees see only their own attendance, leave, payroll and reviews.

## 1.4 Business rules (enforced in the Facade layer)

- BR-01 Email must be unique across users and candidates.
- BR-02 A candidate can apply to a job only once, and only while the job is Open and the deadline has not passed.
- BR-03 Job salary max must be >= salary min; vacancies >= 1; deadline cannot be in the past when created.
- BR-04 Application status changes follow the pipeline. Manual change is limited to Shortlisted / Rejected; Interview, Offered and Hired are set by the interview and offer workflows.
- BR-05 An interview can only be scheduled for a Shortlisted/Interview application, in the future, and the interviewer cannot have another interview within one hour.
- BR-06 Interview score must be 1-10.
- BR-07 An offer can only be issued after interview, once per application, with salary > 0 and joining date not in the past.
- BR-08 Accepting an offer marks the application Hired, creates the Employee (code EMPnnnnn) and sets the user role to Employee. Declining marks it Rejected.
- BR-09 One employee profile per user; salary > 0; an employee cannot be their own manager.
- BR-10 Leave end date >= start date; no overlap with pending/approved leave; yearly balance of the leave type cannot be exceeded; total days = inclusive calendar days.
- BR-11 Only Pending leave can be approved/rejected, and nobody can approve their own leave.
- BR-12 One attendance row per employee per day; check-out must be after check-in; no future dates; check-in after 09:40 = Late.
- BR-13 One payroll row per employee per month. Net = Basic + Allowance - Deduction (default allowance 10 %, deduction 5 % of basic; change in `PayrollRules`).
- BR-14 Review rating 1-5 and an employee cannot review themselves.
- BR-15 A referenced record (department, employee, job ...) cannot be deleted.
- BR-16 Admins cannot deactivate, demote or delete their own account.

## 1.5 Non-functional requirements

- Security: ASP.NET Core cookie authentication, HttpOnly cookie, anti-forgery tokens on every POST, salted password hashing (PBKDF2), role-based authorization, every page requires login unless marked anonymous.
- Maintainability: layered architecture (Entities / DataAccess / Facade / Web.UI) identical to the CRM-Education project; one repository and one facade per entity.
- Usability: responsive Bootstrap 5 UI, validation messages, success / error alerts.
- Data integrity: unique indexes, foreign keys with Restrict, business validation in the Facade layer.
- Performance: AsNoTracking reads, includes only where the screen needs related data.
- Portability: SQL Server (Express) + .NET 10.

## 1.6 Requirement -> module traceability

| Requirement | Entity | Facade | Controller |
|---|---|---|---|
| FR-01..04 | User, Role | UserFacade | AccountController, UsersController |
| FR-05, 06 | JobOpening | JobOpeningFacade | JobsController, CareersController |
| FR-07, 08, 09 | Candidate, JobApplication | CandidateFacade, JobApplicationFacade | CareersController, CandidatesController, ApplicationsController |
| FR-10 | Interview | InterviewFacade | InterviewsController |
| FR-11, 12 | OfferLetter, Employee | OfferLetterFacade | OffersController |
| FR-13, 14 | Department, Designation, Employee | DepartmentFacade, DesignationFacade, EmployeeFacade | DepartmentsController, DesignationsController, EmployeesController |
| FR-15 | Attendance | AttendanceFacade | AttendanceController |
| FR-16 | LeaveType, LeaveRequest | LeaveTypeFacade, LeaveRequestFacade | LeaveTypesController, LeavesController |
| FR-17 | Payroll | PayrollFacade | PayrollController |
| FR-18 | PerformanceReview | PerformanceReviewFacade | ReviewsController |
| FR-19, 20 | - | - | DashboardController, self filters in each controller |
