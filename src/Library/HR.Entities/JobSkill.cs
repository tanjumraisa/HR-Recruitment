using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Skill required by a job (used for matching)
    public class JobSkill : ISoftDelete
    {
        [Key]
        public int JobSkillId { get; set; }

        [Display(Name = "Job opening"), Range(1, int.MaxValue, ErrorMessage = "Please select job opening.")]
        public int JobOpeningId { get; set; }

        [Display(Name = "Skill"), Range(1, int.MaxValue, ErrorMessage = "Please select skill.")]
        public int SkillId { get; set; }

        [Display(Name = "Mandatory")]
        public bool IsMandatory { get; set; }

        [Display(Name = "Min years"), Range(0, 40)]
        public int MinYears { get; set; }

        [Display(Name = "Weight (1-10)"), Range(1, 10)]
        public int Weight { get; set; } = 5;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(JobOpeningId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual JobOpening JobOpening { get; set; } = null!;

        [ForeignKey(nameof(SkillId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Skill Skill { get; set; } = null!;
    }
}
