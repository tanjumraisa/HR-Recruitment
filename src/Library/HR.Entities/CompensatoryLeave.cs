using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Leave earned for holiday work
    public class CompensatoryLeave : ISoftDelete
    {
        [Key]
        public int CompensatoryLeaveId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Worked on holiday")]
        public int? HolidayId { get; set; }

        [Display(Name = "Worked on"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime WorkedOn { get; set; } = DateTime.Today;

        [Display(Name = "Days"), Column(TypeName = "decimal(6, 2)"), Range(typeof(decimal), "0", "10")]
        public decimal Days { get; set; } = 1;

        [Display(Name = "Expiry date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Available|Used|Expired")]
        public string Status { get; set; } = "Available";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(HolidayId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Holiday? Holiday { get; set; }
    }
}
