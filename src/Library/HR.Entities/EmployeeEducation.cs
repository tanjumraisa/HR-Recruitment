using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Academic record
    [DisplayColumn(nameof(Degree))]
    public class EmployeeEducation : ISoftDelete
    {
        [Key]
        public int EmployeeEducationId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Degree"), Required, StringLength(100)]
        public string Degree { get; set; } = string.Empty;

        [Display(Name = "Institution"), Required, StringLength(150)]
        public string Institution { get; set; } = string.Empty;

        [Display(Name = "Board"), StringLength(100)]
        public string? Board { get; set; }

        [Display(Name = "Passing year"), Range(1950, 2100)]
        public int PassingYear { get; set; }

        [Display(Name = "Result"), StringLength(20)]
        public string? Result { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;
    }
}
