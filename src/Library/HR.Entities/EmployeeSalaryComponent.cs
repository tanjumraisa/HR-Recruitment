using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Salary structure of an employee
    public class EmployeeSalaryComponent : ISoftDelete
    {
        [Key]
        public int EmployeeSalaryComponentId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Salary component"), Range(1, int.MaxValue, ErrorMessage = "Please select salary component.")]
        public int SalaryComponentId { get; set; }

        [Display(Name = "Amount"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal Amount { get; set; }

        [Display(Name = "Effective from"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime EffectiveFrom { get; set; } = DateTime.Today;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(SalaryComponentId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual SalaryComponent SalaryComponent { get; set; } = null!;
    }
}
