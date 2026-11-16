using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Score sheet of one evaluator
    public class InterviewFeedback : ISoftDelete
    {
        [Key]
        public int InterviewFeedbackId { get; set; }

        [Display(Name = "Interview"), Range(1, int.MaxValue, ErrorMessage = "Please select interview.")]
        public int InterviewId { get; set; }

        [Display(Name = "Evaluator"), Range(1, int.MaxValue, ErrorMessage = "Please select evaluator.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Technical score"), Range(0, 10)]
        public int TechnicalScore { get; set; }

        [Display(Name = "Communication score"), Range(0, 10)]
        public int CommunicationScore { get; set; }

        [Display(Name = "Attitude score"), Range(0, 10)]
        public int AttitudeScore { get; set; }

        [Display(Name = "Recommendation"), Required, StringLength(20), Choices("StrongHire|Hire|Hold|Reject")]
        public string Recommendation { get; set; } = "Hold";

        [Display(Name = "Comments"), StringLength(2000), DataType(DataType.MultilineText)]
        public string? Comments { get; set; }

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
