using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Permanent, contractual, intern ...
    [DisplayColumn(nameof(Name))]
    public class EmploymentType : ISoftDelete
    {
        [Key]
        public int EmploymentTypeId { get; set; }

        [Display(Name = "Name"), Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description"), StringLength(250)]
        public string? Description { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}
