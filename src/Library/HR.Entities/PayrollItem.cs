using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Line of a payslip
    public class PayrollItem : ISoftDelete
    {
        [Key]
        public int PayrollItemId { get; set; }

        [Display(Name = "Payroll"), Range(1, int.MaxValue, ErrorMessage = "Please select payroll.")]
        public int PayrollId { get; set; }

        [Display(Name = "Salary component"), Range(1, int.MaxValue, ErrorMessage = "Please select salary component.")]
        public int SalaryComponentId { get; set; }

        [Display(Name = "Amount"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal Amount { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(PayrollId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Payroll Payroll { get; set; } = null!;

        [ForeignKey(nameof(SalaryComponentId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual SalaryComponent SalaryComponent { get; set; } = null!;
    }
}
