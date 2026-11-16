using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Skill master (used by employees, candidates, jobs)
    [DisplayColumn(nameof(Name))]
    public class Skill : ISoftDelete
    {
        [Key]
        public int SkillId { get; set; }

        [Display(Name = "Name"), Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Category"), Required, StringLength(30), Choices("Technical|Soft|Language|Domain")]
        public string Category { get; set; } = "Technical";

        [Display(Name = "Description"), StringLength(250)]
        public string? Description { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}
