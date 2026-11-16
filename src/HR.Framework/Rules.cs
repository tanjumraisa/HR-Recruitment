namespace HR.Framework
{
    public static class PayrollRules
    {
        public const decimal AllowancePercent = 10m;   // house + medical allowance on basic
        public const decimal DeductionPercent = 5m;    // tax / provident fund on basic
    }

    public static class AttendanceRules
    {
        public static readonly TimeSpan OfficeStart = new(9, 30, 0);
        public static readonly TimeSpan GracePeriod = TimeSpan.FromMinutes(10);
    }
}
