namespace HR.Framework
{
    public static class Extentions
    {
        public static string ToDateText(this DateTime? datetime) =>
            datetime.HasValue ? datetime.Value.ToString("dd/MM/yyyy") : "-";

        public static string ToEmployeeCode(this int id) => $"EMP{id:D5}";

        public static string ToMonthName(this int month) =>
            month is >= 1 and <= 12
                ? System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month)
                : "-";
    }
}
