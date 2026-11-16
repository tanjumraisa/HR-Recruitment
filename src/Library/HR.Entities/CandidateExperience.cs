using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Candidate work history
    [DisplayColumn(nameof(CompanyName))]
    public class CandidateExperience : ISoftDelete
    {
        [Key]
        public int CandidateExperienceId { get; set; }

        [Display(Name = "Candidate"), Range(1, int.MaxValue, ErrorMessage = "Please select candidate.")]
        public int CandidateId { get; set; }

        [Display(Name = "Company name"), Required, StringLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        [Display(Name = "Position"), StringLength(100)]
        public string? Position { get; set; }

        [Display(Name = "From date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime FromDate { get; set; } = DateTime.Today;

        [Display(Name = "To date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Responsibilities"), StringLength(2000), DataType(DataType.MultilineText)]
        public string? Responsibilities { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(CandidateId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Candidate Candidate { get; set; } = null!;
    }
}
