using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Loan or salary advance
    [DisplayColumn(nameof(ReferenceNo))]
    public class LoanAdvance : ISoftDelete
    {
        [Key]
        public int LoanAdvanceId { get; set; }

        [Display(Name = "Reference no"), Required, StringLength(30)]
        public string ReferenceNo { get; set; } = string.Empty;

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Loan type"), Required, StringLength(20), Choices("Loan|Advance")]
        public string LoanType { get; set; } = "Loan";

        [Display(Name = "Amount"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal Amount { get; set; }

        [Display(Name = "Installments"), Range(1, 120)]
        public int Installments { get; set; } = 1;

        [Display(Name = "Installment amount"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal InstallmentAmount { get; set; }

        [Display(Name = "Paid so far"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal PaidAmount { get; set; }

        [Display(Name = "Issue date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime IssueDate { get; set; } = DateTime.Today;

        [Display(Name = "Status"), Required, StringLength(20), Choices("Running|Closed")]
        public string Status { get; set; } = "Running";

        [Display(Name = "Purpose"), StringLength(250)]
        public string? Purpose { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;
    }
}
