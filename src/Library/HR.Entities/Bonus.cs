using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Bonus payment
    public class Bonus : ISoftDelete
    {
        [Key]
        public int BonusId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Bonus type"), Required, StringLength(20), Choices("Festival|Performance|Annual|Other")]
        public string BonusType { get; set; } = "Festival";

        [Display(Name = "Amount"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal Amount { get; set; }

        [Display(Name = "Month"), Range(1, 12)]
        public int Month { get; set; }

        [Display(Name = "Year"), Range(2000, 2100)]
        public int Year { get; set; }

        [Display(Name = "Paid in payroll")]
        public int? PayrollId { get; set; }

        [Display(Name = "Remarks"), StringLength(250)]
        public string? Remarks { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(PayrollId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Payroll? Payroll { get; set; }
    }
}
