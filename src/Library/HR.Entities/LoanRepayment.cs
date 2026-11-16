using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Installment paid
    public class LoanRepayment : ISoftDelete
    {
        [Key]
        public int LoanRepaymentId { get; set; }

        [Display(Name = "Loan"), Range(1, int.MaxValue, ErrorMessage = "Please select loan.")]
        public int LoanAdvanceId { get; set; }

        [Display(Name = "Deducted in payroll")]
        public int? PayrollId { get; set; }

        [Display(Name = "Amount"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal Amount { get; set; }

        [Display(Name = "Paid on"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime PaidOn { get; set; } = DateTime.Today;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(LoanAdvanceId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual LoanAdvance LoanAdvance { get; set; } = null!;

        [ForeignKey(nameof(PayrollId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Payroll? Payroll { get; set; }
    }
}
