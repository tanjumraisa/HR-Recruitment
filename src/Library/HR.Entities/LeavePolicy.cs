using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Leave entitlement rule
    public class LeavePolicy : ISoftDelete
    {
        [Key]
        public int LeavePolicyId { get; set; }

        [Display(Name = "Leave type"), Range(1, int.MaxValue, ErrorMessage = "Please select leave type.")]
        public int LeaveTypeId { get; set; }

        [Display(Name = "Grade (empty = all)")]
        public int? GradeId { get; set; }

        [Display(Name = "Days per year"), Range(0, 365)]
        public int DaysPerYear { get; set; }

        [Display(Name = "Carry forward max"), Range(0, 365)]
        public int CarryForwardMax { get; set; }

        [Display(Name = "Encashable")]
        public bool IsEncashable { get; set; }

        [Display(Name = "Min service months"), Range(0, 120)]
        public int MinServiceMonths { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(LeaveTypeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual LeaveType LeaveType { get; set; } = null!;

        [ForeignKey(nameof(GradeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Grade? Grade { get; set; }
    }
}
