using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Request to cancel an approved leave
    public class LeaveWithdrawal : ISoftDelete
    {
        [Key]
        public int LeaveWithdrawalId { get; set; }

        [Display(Name = "Leave"), Range(1, int.MaxValue, ErrorMessage = "Please select leave.")]
        public int LeaveRequestId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Reason"), Required, StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "Status"), Required, StringLength(20), Choices("Pending|Approved|Rejected")]
        public string Status { get; set; } = "Pending";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(LeaveRequestId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual LeaveRequest LeaveRequest { get; set; } = null!;

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;
    }
}
