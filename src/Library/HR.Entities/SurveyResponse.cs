using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Answer of one employee
    public class SurveyResponse : ISoftDelete
    {
        [Key]
        public int SurveyResponseId { get; set; }

        [Display(Name = "Question"), Range(1, int.MaxValue, ErrorMessage = "Please select question.")]
        public int SurveyQuestionId { get; set; }

        [Display(Name = "Employee (empty = anonymous)")]
        public int? EmployeeId { get; set; }

        [Display(Name = "Rating"), Range(1, 5)]
        public int? Rating { get; set; }

        [Display(Name = "Answer"), StringLength(500)]
        public string? Answer { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(SurveyQuestionId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual SurveyQuestion SurveyQuestion { get; set; } = null!;

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee? Employee { get; set; }
    }
}
