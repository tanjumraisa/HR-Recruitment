using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Shift roster of an employee
    public class EmployeeShift : ISoftDelete
    {
        [Key]
        public int EmployeeShiftId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Shift"), Range(1, int.MaxValue, ErrorMessage = "Please select shift.")]
        public int ShiftId { get; set; }

        [Display(Name = "From date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime FromDate { get; set; } = DateTime.Today;

        [Display(Name = "To date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(ShiftId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Shift Shift { get; set; } = null!;
    }
}
