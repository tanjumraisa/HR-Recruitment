namespace HR.Framework
{
    public static class JobStatus
    {
        public const string Draft = "Draft";
        public const string Open = "Open";
        public const string Closed = "Closed";
        public static readonly string[] All = { Draft, Open, Closed };
    }

    public static class ApplicationStatus
    {
        public const string Applied = "Applied";
        public const string Shortlisted = "Shortlisted";
        public const string Interview = "Interview";
        public const string Offered = "Offered";
        public const string Hired = "Hired";
        public const string Rejected = "Rejected";
        public static readonly string[] All = { Applied, Shortlisted, Interview, Offered, Hired, Rejected };

        private static readonly Dictionary<string, string[]> Flow = new()
        {
            [Applied] = new[] { Shortlisted, Rejected },
            [Shortlisted] = new[] { Interview, Rejected },
            [Interview] = new[] { Offered, Rejected },
            [Offered] = new[] { Hired, Rejected },
            [Hired] = Array.Empty<string>(),
            [Rejected] = Array.Empty<string>()
        };

        public static bool CanMove(string from, string to) =>
            from == to || (Flow.TryGetValue(from, out var next) && next.Contains(to));

        public static string[] NextOf(string from) =>
            Flow.TryGetValue(from, out var next) ? next : Array.Empty<string>();
    }

    public static class InterviewStatus
    {
        public const string Scheduled = "Scheduled";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
        public static readonly string[] All = { Scheduled, Completed, Cancelled };
        public static readonly string[] Modes = { "Onsite", "Online", "Phone" };
    }

    public static class OfferStatus
    {
        public const string Issued = "Issued";
        public const string Accepted = "Accepted";
        public const string Declined = "Declined";
        public static readonly string[] All = { Issued, Accepted, Declined };
    }

    public static class LeaveStatus
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Cancelled = "Cancelled";
        public static readonly string[] All = { Pending, Approved, Rejected, Cancelled };
    }

    public static class AttendanceStatus
    {
        public const string Present = "Present";
        public const string Absent = "Absent";
        public const string Late = "Late";
        public const string OnLeave = "OnLeave";
        public static readonly string[] All = { Present, Absent, Late, OnLeave };
    }

    public static class EmployeeStatus
    {
        public const string Active = "Active";
        public const string OnLeave = "OnLeave";
        public const string Terminated = "Terminated";
        public static readonly string[] All = { Active, OnLeave, Terminated };
    }

    public static class PayrollStatus
    {
        public const string Generated = "Generated";
        public const string Paid = "Paid";
        public static readonly string[] All = { Generated, Paid };
    }
}
