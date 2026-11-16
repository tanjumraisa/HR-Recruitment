using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Training course
    [DisplayColumn(nameof(Title))]
    public class TrainingProgram : ISoftDelete
    {
        [Key]
        public int TrainingProgramId { get; set; }

        [Display(Name = "Title"), Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Skill developed")]
        public int? SkillId { get; set; }

        [Display(Name = "Trainer"), StringLength(100)]
        public string? Trainer { get; set; }

        [Display(Name = "Venue"), StringLength(150)]
        public string? Venue { get; set; }

        [Display(Name = "Start date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Display(Name = "End date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today;

        [Display(Name = "Capacity"), Range(1, 1000)]
        public int Capacity { get; set; } = 20;

        [Display(Name = "Cost"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal Cost { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Planned|Running|Completed|Cancelled")]
        public string Status { get; set; } = "Planned";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(SkillId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Skill? Skill { get; set; }
    }
}
