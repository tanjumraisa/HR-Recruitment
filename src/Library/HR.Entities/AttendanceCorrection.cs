using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Request to fix a punch
    public class AttendanceCorrection : ISoftDelete
    {
        [Key]
        public int AttendanceCorrectionId { get; set; }

        [Display(Name = "Attendance"), Range(1, int.MaxValue, ErrorMessage = "Please select attendance.")]
        public int AttendanceId { get; set; }

        [Display(Name = "Requested by"), Range(1, int.MaxValue, ErrorMessage = "Please select requested by.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Requested check in"), DataType(DataType.Time)]
        public TimeSpan? RequestedCheckIn { get; set; }

        [Display(Name = "Requested check out"), DataType(DataType.Time)]
        public TimeSpan? RequestedCheckOut { get; set; }

        [Display(Name = "Reason"), Required, StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "Status"), Required, StringLength(20), Choices("Pending|Approved|Rejected")]
        public string Status { get; set; } = "Pending";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(AttendanceId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Attendance Attendance { get; set; } = null!;

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;
    }
}
