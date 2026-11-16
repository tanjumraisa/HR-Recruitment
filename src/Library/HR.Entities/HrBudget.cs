using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // HR budget per department
    public class HrBudget : ISoftDelete
    {
        [Key]
        public int HrBudgetId { get; set; }

        [Display(Name = "Financial year"), Range(1, int.MaxValue, ErrorMessage = "Please select financial year.")]
        public int FinancialYearId { get; set; }

        [Display(Name = "Department"), Range(1, int.MaxValue, ErrorMessage = "Please select department.")]
        public int DepartmentId { get; set; }

        [Display(Name = "Category"), Required, StringLength(20), Choices("Recruitment|Training|Salary|Welfare|Other")]
        public string Category { get; set; } = "Training";

        [Display(Name = "Budget amount"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal BudgetAmount { get; set; }

        [Display(Name = "Spent amount"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal SpentAmount { get; set; }

        [Display(Name = "Remarks"), StringLength(250)]
        public string? Remarks { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(FinancialYearId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual FinancialYear FinancialYear { get; set; } = null!;

        [ForeignKey(nameof(DepartmentId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Department Department { get; set; } = null!;
    }
}
