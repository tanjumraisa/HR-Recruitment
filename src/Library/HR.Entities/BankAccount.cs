using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Salary bank account
    [DisplayColumn(nameof(AccountNumber))]
    public class BankAccount : ISoftDelete
    {
        [Key]
        public int BankAccountId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Bank name"), Required, StringLength(100)]
        public string BankName { get; set; } = string.Empty;

        [Display(Name = "Branch name"), StringLength(100)]
        public string? BranchName { get; set; }

        [Display(Name = "Account name"), StringLength(100)]
        public string? AccountName { get; set; }

        [Display(Name = "Account number"), Required, StringLength(30)]
        public string AccountNumber { get; set; } = string.Empty;

        [Display(Name = "Routing number"), StringLength(20)]
        public string? RoutingNumber { get; set; }

        [Display(Name = "Salary account")]
        public bool IsPrimary { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;
    }
}
