using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Interview board member
    public class InterviewPanel : ISoftDelete
    {
        [Key]
        public int InterviewPanelId { get; set; }

        [Display(Name = "Interview"), Range(1, int.MaxValue, ErrorMessage = "Please select interview.")]
        public int InterviewId { get; set; }

        [Display(Name = "Panel member"), Range(1, int.MaxValue, ErrorMessage = "Please select panel member.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Panel role"), Required, StringLength(20), Choices("Chair|Member|Observer")]
        public string PanelRole { get; set; } = "Member";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(InterviewId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Interview Interview { get; set; } = null!;

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;
    }
}
