using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Skill of an employee
    public class EmployeeSkill : ISoftDelete
    {
        [Key]
        public int EmployeeSkillId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Skill"), Range(1, int.MaxValue, ErrorMessage = "Please select skill.")]
        public int SkillId { get; set; }

        [Display(Name = "Proficiency"), Required, StringLength(20), Choices("Beginner|Intermediate|Advanced|Expert")]
        public string Proficiency { get; set; } = "Intermediate";

        [Display(Name = "Years of experience"), Range(0, 60)]
        public int YearsOfExperience { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(SkillId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Skill Skill { get; set; } = null!;
    }
}
