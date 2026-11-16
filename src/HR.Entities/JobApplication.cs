using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    public class JobApplication : ISoftDelete
    {
        [Key]
        public int ApplicationId { get; set; }

        public int JobOpeningId { get; set; }

        public int CandidateId { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        [Required, StringLength(20)]
        public string Status { get; set; } = ApplicationStatus.Applied;

        [StringLength(2000)]
        public string? CoverLetter { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(JobOpeningId))]
        public virtual JobOpening JobOpening { get; set; } = null!;
        [ForeignKey(nameof(CandidateId))]
        public virtual Candidate Candidate { get; set; } = null!;
        public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
        public virtual OfferLetter? Offer { get; set; }
    }
}
