using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Question of a survey
    [DisplayColumn(nameof(QuestionText))]
    public class SurveyQuestion : ISoftDelete
    {
        [Key]
        public int SurveyQuestionId { get; set; }

        [Display(Name = "Survey"), Range(1, int.MaxValue, ErrorMessage = "Please select survey.")]
        public int SurveyId { get; set; }

        [Display(Name = "Question text"), Required, StringLength(300)]
        public string QuestionText { get; set; } = string.Empty;

        [Display(Name = "Question type"), Required, StringLength(20), Choices("Rating|YesNo|Text")]
        public string QuestionType { get; set; } = "Rating";

        [Display(Name = "Sort order"), Range(0, 100)]
        public int SortOrder { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(SurveyId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Survey Survey { get; set; } = null!;
    }
}
