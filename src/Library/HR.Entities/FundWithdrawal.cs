using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Withdrawal / settlement from a fund
    public class FundWithdrawal : ISoftDelete
    {
        [Key]
        public int FundWithdrawalId { get; set; }

        [Display(Name = "Member"), Range(1, int.MaxValue, ErrorMessage = "Please select member.")]
        public int FundMembershipId { get; set; }

        [Display(Name = "Request date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime RequestDate { get; set; } = DateTime.Today;

        [Display(Name = "Amount"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal Amount { get; set; }

        [Display(Name = "Reason"), Required, StringLength(250)]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "Status"), Required, StringLength(20), Choices("Pending|Approved|Rejected|Paid")]
        public string Status { get; set; } = "Pending";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(FundMembershipId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual FundMembership FundMembership { get; set; } = null!;
    }
}
