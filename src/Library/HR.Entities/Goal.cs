using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // KPI / objective
    [DisplayColumn(nameof(Title))]
    public class Goal : ISoftDelete
    {
        [Key]
        public int GoalId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Review cycle"), Range(1, int.MaxValue, ErrorMessage = "Please select review cycle.")]
        public int ReviewCycleId { get; set; }

        [Display(Name = "Title"), Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Description"), StringLength(2000), DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Display(Name = "Weight"), Range(0, 100)]
        public int Weight { get; set; }

        [Display(Name = "Progress percent"), Range(0, 100)]
        public int ProgressPercent { get; set; }

        [Display(Name = "Due date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.Today;

        [Display(Name = "Status"), Required, StringLength(20), Choices("NotStarted|InProgress|Completed")]
        public string Status { get; set; } = "NotStarted";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(ReviewCycleId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ReviewCycle ReviewCycle { get; set; } = null!;
    }
}
