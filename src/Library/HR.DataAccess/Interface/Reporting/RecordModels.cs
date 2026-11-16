namespace HR.DataAccess.Reporting
{
    // Read-only models used by the "connected records" page and the reports.

    public class RecordField
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? LinkEntity { get; set; }
        public int? LinkId { get; set; }
    }

    public class RecordRow
    {
        public int Id { get; set; }
        public List<string> Cells { get; set; } = new();
    }

    public class RecordChildGroup
    {
        public string EntityName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ForeignKey { get; set; } = string.Empty;
        public int Total { get; set; }
        public List<string> Columns { get; set; } = new();
        public List<RecordRow> Rows { get; set; } = new();
    }

    public class RecordDetails
    {
        public string EntityName { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<RecordField> Fields { get; set; } = new();
        public List<RecordChildGroup> Children { get; set; } = new();
    }

    public class TableSummary
    {
        public string EntityName { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public int Rows { get; set; }
        public int Columns { get; set; }
        public bool SoftDelete { get; set; }
        public List<string> References { get; set; } = new();
        public List<string> ReferencedBy { get; set; } = new();
    }

    public class ReportResult
    {
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public List<string> Columns { get; set; } = new();
        public List<string[]> Rows { get; set; } = new();
        public string[]? Footer { get; set; }
    }

    public class ReportFilter
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? BranchId { get; set; }
        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }
        public string? Status { get; set; }
        public string? Name { get; set; }
        public string? EmployeeCode { get; set; }
    }

    public class DashboardStats
    {
        public int Employees { get; set; }
        public int Departments { get; set; }
        public int Branches { get; set; }
        public int OpenJobs { get; set; }
        public int Applications { get; set; }
        public int PendingLeaves { get; set; }
        public int PresentToday { get; set; }
        public int LateToday { get; set; }
        public int OnLeaveToday { get; set; }
        public int PendingRequests { get; set; }
        public int OpenRequisitions { get; set; }
        public int RunningTrainings { get; set; }
        public int ContractsExpiring { get; set; }
        public int RunningLoans { get; set; }
        public List<(string Name, DateTime Date)> UpcomingHolidays { get; set; } = new();
        public Dictionary<string, int> HeadcountByDepartment { get; set; } = new();
    }

    public class EmployeeSearchRow
    {
        public int EmployeeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
    }

    // Numbers for the chart dashboard (like the Mojuri HRM home page).
    public class DashboardCharts
    {
        public DateTime Date { get; set; }
        public int TotalEmployees { get; set; }
        public int DaysPresent { get; set; }
        public int DaysAbsent { get; set; }
        public int UnscheduledLeave { get; set; }
        public int SickLeaveDays { get; set; }
        public int CasualLeaveDays { get; set; }
        public decimal AbsenteeismRate { get; set; }
        public decimal PresentismRate { get; set; }
        public List<(string Label, int Value)> ExperienceBuckets { get; set; } = new();
        public List<(string Label, int Value)> AbsencesByMonth { get; set; } = new();
        public List<(string Label, int Value)> WorkLocations { get; set; } = new();
        public List<(string Label, int Value)> LeaveTypes { get; set; } = new();
        public List<(string Label, decimal Percent)> AttendanceByDepartment { get; set; } = new();
    }
}
