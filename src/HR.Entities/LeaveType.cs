using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    public class LeaveType : ISoftDelete
    {
        [Key]
        public int LeaveTypeId { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public int DaysPerYear { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    }
}
