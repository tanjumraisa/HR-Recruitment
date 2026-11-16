using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Extra working hours
    public class OvertimeRecord : ISoftDelete
    {
        [Key]
        public int OvertimeRecordId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Attendance")]
        public int? AttendanceId { get; set; }

        [Display(Name = "Date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Display(Name = "Hours"), Column(TypeName = "decimal(6, 2)"), Range(typeof(decimal), "0", "24")]
        public decimal Hours { get; set; }

        [Display(Name = "Rate per hour"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal RatePerHour { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Pending|Approved|Rejected|Paid")]
        public string Status { get; set; } = "Pending";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(AttendanceId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Attendance? Attendance { get; set; }
    }
}
