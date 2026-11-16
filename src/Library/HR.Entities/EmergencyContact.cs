using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Person to call in an emergency
    [DisplayColumn(nameof(Name))]
    public class EmergencyContact : ISoftDelete
    {
        [Key]
        public int EmergencyContactId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Name"), Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Relationship"), StringLength(50)]
        public string? Relationship { get; set; }

        [Display(Name = "Phone"), Required, StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Address"), StringLength(250)]
        public string? Address { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;
    }
}
