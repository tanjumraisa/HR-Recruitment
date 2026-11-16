# 2. ER Diagram

Entity-Relationship diagram of the HR & Recruitment system (Mermaid). It renders on GitHub, GitLab, VS Code (Markdown Preview Mermaid extension) and https://mermaid.live .

```mermaid
erDiagram
    Role ||--o{ User : "has"
    User ||--o| Employee : "is"
    User |o--o| Candidate : "may be"
    User ||--o{ JobOpening : "posts"
    Department ||--o{ Employee : "contains"
    Designation ||--o{ Employee : "held by"
    Department ||--o{ JobOpening : "opens"
    Designation ||--o{ JobOpening : "for"
    Employee |o--o{ Employee : "manages"
    JobOpening ||--o{ JobApplication : "receives"
    Candidate ||--o{ JobApplication : "submits"
    JobApplication ||--o{ Interview : "has"
    Employee ||--o{ Interview : "conducts"
    JobApplication ||--o| OfferLetter : "results in"
    Employee ||--o{ LeaveRequest : "requests"
    LeaveType ||--o{ LeaveRequest : "categorises"
    Employee ||--o{ Attendance : "records"
    Employee ||--o{ Payroll : "paid via"
    Employee ||--o{ PerformanceReview : "reviewed in"

    Role {
        int RoleId PK
        string RoleName UK
    }
    User {
        int UserId PK
        string FullName
        string Email UK
        string PasswordHash
        string PhoneNumber
        int RoleId FK
        boolean IsActive
        datetime CreatedAt
    }
    Department {
        int DepartmentId PK
        string Name UK
        string Description
        boolean IsActive
    }
    Designation {
        int DesignationId PK
        string Title UK
        decimal BaseSalary
    }
    Employee {
        int EmployeeId PK
        string EmployeeCode UK
        int UserId FK
        int DepartmentId FK
        int DesignationId FK
        int ManagerId FK
        datetime JoinDate
        decimal Salary
        string Status
        string Address
    }
    JobOpening {
        int JobOpeningId PK
        string Title
        int DepartmentId FK
        int DesignationId FK
        int Vacancies
        string Description
        string Requirements
        decimal SalaryMin
        decimal SalaryMax
        string EmploymentType
        datetime Deadline
        string Status
        int PostedById FK
        datetime CreatedAt
    }
    Candidate {
        int CandidateId PK
        int UserId FK
        string FullName
        string Email UK
        string Phone
        string Skills
        int ExperienceYears
        string ResumePath
        datetime CreatedAt
    }
    JobApplication {
        int ApplicationId PK
        int JobOpeningId FK
        int CandidateId FK
        datetime AppliedAt
        string Status
        string CoverLetter
        string Notes
    }
    Interview {
        int InterviewId PK
        int ApplicationId FK
        int InterviewerId FK
        datetime ScheduledAt
        string Mode
        string Location
        string Status
        int Score
        string Feedback
    }
    OfferLetter {
        int OfferId PK
        int ApplicationId FK
        decimal OfferedSalary
        datetime JoiningDate
        string Status
        datetime IssuedAt
    }
    LeaveType {
        int LeaveTypeId PK
        string Name UK
        int DaysPerYear
    }
    LeaveRequest {
        int LeaveRequestId PK
        int EmployeeId FK
        int LeaveTypeId FK
        datetime FromDate
        datetime ToDate
        int TotalDays
        string Reason
        string Status
        int ApprovedById FK
        datetime AppliedAt
    }
    Attendance {
        int AttendanceId PK
        int EmployeeId FK
        datetime Date
        time CheckIn
        time CheckOut
        string Status
    }
    Payroll {
        int PayrollId PK
        int EmployeeId FK
        int Month
        int Year
        decimal BasicSalary
        decimal Allowance
        decimal Deduction
        decimal NetSalary
        string Status
        datetime GeneratedAt
    }
    PerformanceReview {
        int ReviewId PK
        int EmployeeId FK
        int ReviewerId FK
        string Period
        int Rating
        string Comments
        datetime ReviewedAt
    }
```

## Relationship summary

| Parent | Child | Type | Meaning |
|---|---|---|---|
| Role | User | 1 : N | Role has User |
| User | Employee | 1 : 0..1 | User is Employee |
| User | Candidate | 0..1 : 0..1 | User may be Candidate |
| User | JobOpening | 1 : N | User posts JobOpening |
| Department | Employee | 1 : N | Department contains Employee |
| Designation | Employee | 1 : N | Designation held by Employee |
| Department | JobOpening | 1 : N | Department opens JobOpening |
| Designation | JobOpening | 1 : N | Designation for JobOpening |
| Employee | Employee | 0..1 : N | Employee manages Employee |
| JobOpening | JobApplication | 1 : N | JobOpening receives JobApplication |
| Candidate | JobApplication | 1 : N | Candidate submits JobApplication |
| JobApplication | Interview | 1 : N | JobApplication has Interview |
| Employee | Interview | 1 : N | Employee conducts Interview |
| JobApplication | OfferLetter | 1 : 0..1 | JobApplication results in OfferLetter |
| Employee | LeaveRequest | 1 : N | Employee requests LeaveRequest |
| LeaveType | LeaveRequest | 1 : N | LeaveType categorises LeaveRequest |
| Employee | Attendance | 1 : N | Employee records Attendance |
| Employee | Payroll | 1 : N | Employee paid via Payroll |
| Employee | PerformanceReview | 1 : N | Employee reviewed in PerformanceReview |

## Notes

- `User` is the login account. A user may own one `Employee` profile and/or one `Candidate` profile.
- `Employee.ManagerId` is a self reference (manager / subordinate).
- `LeaveRequest` has two links to `Employee`: the requester (`EmployeeId`) and the approver (`ApprovedById`).
- `PerformanceReview` links to `Employee` twice: the reviewed employee and the reviewer.
- `OfferLetter` is 1:1 with `JobApplication`, so one application can have at most one offer.
