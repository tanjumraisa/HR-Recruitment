using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Candidate academic record
    [DisplayColumn(nameof(Degree))]
    public class CandidateEducation : ISoftDelete
    {
        [Key]
        public int CandidateEducationId { get; set; }

        [Display(Name = "Candidate"), Range(1, int.MaxValue, ErrorMessage = "Please select candidate.")]
        public int CandidateId { get; set; }

        [Display(Name = "Degree"), Required, StringLength(100)]
        public string Degree { get; set; } = string.Empty;

        [Display(Name = "Institution"), Required, StringLength(150)]
        public string Institution { get; set; } = string.Empty;

        [Display(Name = "Passing year"), Range(1950, 2100)]
        public int PassingYear { get; set; }

        [Display(Name = "Result"), StringLength(20)]
        public string? Result { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(CandidateId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Candidate Candidate { get; set; } = null!;
    }
}
