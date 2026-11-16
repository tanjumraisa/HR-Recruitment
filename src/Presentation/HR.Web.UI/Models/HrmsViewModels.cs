using HR.DataAccess.Reporting;
using HR.Facade.Interface;
using HR.Web.UI.Helpers;

namespace HR.Web.UI.Models
{
    public class CrudIndexViewModel
    {
        public EntityMeta Meta { get; set; } = null!;
        public ModuleInfo? Module { get; set; }
        public List<object> Items { get; set; } = new();
        public Dictionary<string, Dictionary<int, string>> Lookups { get; set; } = new();
        public Dictionary<string, int> Filters { get; set; } = new();
        public string? Q { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int Total { get; set; }
    }

    public class CrudFormViewModel
    {
        public EntityMeta Meta { get; set; } = null!;
        public ModuleInfo? Module { get; set; }
        public object Entity { get; set; } = null!;
        public bool IsNew { get; set; }
        public Dictionary<string, Dictionary<int, string>> Lookups { get; set; } = new();
        public string? ReturnUrl { get; set; }
    }

    public class ReportViewModel
    {
        public ReportInfo Info { get; set; } = null!;
        public ReportResult Result { get; set; } = null!;
        public ReportFilter Filter { get; set; } = new();
        public Dictionary<int, string> Branches { get; set; } = new();
        public Dictionary<int, string> Departments { get; set; } = new();
        public Dictionary<int, string> Designations { get; set; } = new();
    }

    public class HrmsDashboardViewModel : DashboardViewModel
    {
        public DashboardStats Stats { get; set; } = new();
        public DashboardCharts Charts { get; set; } = new();
        public DateTime Date { get; set; } = DateTime.Today;
        public int? DepartmentId { get; set; }
        public ReportFilter Filter { get; set; } = new();
        public bool Searched { get; set; }
        public List<EmployeeSearchRow> Results { get; set; } = new();
        public Dictionary<int, string> Branches { get; set; } = new();
        public Dictionary<int, string> Departments { get; set; } = new();
        public Dictionary<int, string> Designations { get; set; } = new();
    }
}
