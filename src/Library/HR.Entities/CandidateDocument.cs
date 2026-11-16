using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Candidate document
    [DisplayColumn(nameof(Title))]
    public class CandidateDocument : ISoftDelete
    {
        [Key]
        public int CandidateDocumentId { get; set; }

        [Display(Name = "Candidate"), Range(1, int.MaxValue, ErrorMessage = "Please select candidate.")]
        public int CandidateId { get; set; }

        [Display(Name = "Document type"), Required, StringLength(20), Choices("CV|Certificate|Portfolio|NID|Other")]
        public string DocumentType { get; set; } = "Certificate";

        [Display(Name = "Title"), Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "File name / link"), StringLength(200)]
        public string? Reference { get; set; }

        [Display(Name = "Remarks"), StringLength(250)]
        public string? Remarks { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(CandidateId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Candidate Candidate { get; set; } = null!;
    }
}
