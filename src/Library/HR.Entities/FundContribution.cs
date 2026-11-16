using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Monthly contribution to a fund
    public class FundContribution : ISoftDelete
    {
        [Key]
        public int FundContributionId { get; set; }

        [Display(Name = "Member"), Range(1, int.MaxValue, ErrorMessage = "Please select member.")]
        public int FundMembershipId { get; set; }

        [Display(Name = "Payroll")]
        public int? PayrollId { get; set; }

        [Display(Name = "Month"), Range(1, 12)]
        public int Month { get; set; }

        [Display(Name = "Year"), Range(2000, 2100)]
        public int Year { get; set; }

        [Display(Name = "Employee amount"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal EmployeeAmount { get; set; }

        [Display(Name = "Employer amount"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal EmployerAmount { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(FundMembershipId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual FundMembership FundMembership { get; set; } = null!;

        [ForeignKey(nameof(PayrollId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Payroll? Payroll { get; set; }
    }
}
