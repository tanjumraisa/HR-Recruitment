using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Daily attendance of outsourced worker
    public class OutsourcedAttendance : ISoftDelete
    {
        [Key]
        public int OutsourcedAttendanceId { get; set; }

        [Display(Name = "Worker"), Range(1, int.MaxValue, ErrorMessage = "Please select worker.")]
        public int OutsourcedEmployeeId { get; set; }

        [Display(Name = "Date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Display(Name = "Check in"), DataType(DataType.Time)]
        public TimeSpan? CheckIn { get; set; }

        [Display(Name = "Check out"), DataType(DataType.Time)]
        public TimeSpan? CheckOut { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Present|Absent|Leave")]
        public string Status { get; set; } = "Present";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(OutsourcedEmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual OutsourcedEmployee OutsourcedEmployee { get; set; } = null!;
    }
}
