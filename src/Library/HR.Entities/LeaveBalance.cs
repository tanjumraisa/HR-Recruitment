using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Yearly leave balance
    public class LeaveBalance : ISoftDelete
    {
        [Key]
        public int LeaveBalanceId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Leave type"), Range(1, int.MaxValue, ErrorMessage = "Please select leave type.")]
        public int LeaveTypeId { get; set; }

        [Display(Name = "Year"), Range(2000, 2100)]
        public int Year { get; set; }

        [Display(Name = "Entitled"), Range(0, 365)]
        public int Entitled { get; set; }

        [Display(Name = "Carried forward"), Range(0, 365)]
        public int CarriedForward { get; set; }

        [Display(Name = "Taken"), Range(0, 365)]
        public int Taken { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(LeaveTypeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual LeaveType LeaveType { get; set; } = null!;
    }
}
