using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Team inside a department
    [DisplayColumn(nameof(Name))]
    public class Team : ISoftDelete
    {
        [Key]
        public int TeamId { get; set; }

        [Display(Name = "Name"), Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Department"), Range(1, int.MaxValue, ErrorMessage = "Please select department.")]
        public int DepartmentId { get; set; }

        [Display(Name = "Team lead")]
        public int? LeadEmployeeId { get; set; }

        [Display(Name = "Description"), StringLength(250)]
        public string? Description { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Department Department { get; set; } = null!;

        [ForeignKey(nameof(LeadEmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee? LeadEmployee { get; set; }
    }
}
