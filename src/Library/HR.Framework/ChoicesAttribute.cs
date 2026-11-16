namespace HR.Framework
{
    // Fixed list of allowed values for a text column (shown as a dropdown in the forms).
    [AttributeUsage(AttributeTargets.Property)]
    public class ChoicesAttribute : Attribute
    {
        public ChoicesAttribute(string values)
        {
            Values = values.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        public string[] Values { get; }
    }

    // Status values shared by the HRMS request tables (WFH, shift swap, correction, requisition ...).
    public static class RequestStatus
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
    }
}
