using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    public class LeaveRequest : ISoftDelete
    {
        [Key]
        public int LeaveRequestId { get; set; }

        public int EmployeeId { get; set; }

        public int LeaveTypeId { get; set; }

        [Column(TypeName = "date")]
        public DateTime FromDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime ToDate { get; set; }

        public int TotalDays { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = LeaveStatus.Pending;

        public int? ApprovedById { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; } = null!;
        [ForeignKey(nameof(LeaveTypeId))]
        public virtual LeaveType LeaveType { get; set; } = null!;
        [ForeignKey(nameof(ApprovedById))]
        public virtual Employee? ApprovedBy { get; set; }
    }
}
