using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    public class Candidate : ISoftDelete
    {
        [Key]
        public int CandidateId { get; set; }

        public int? UserId { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(500)]
        public string? Skills { get; set; }

        public int ExperienceYears { get; set; }

        [StringLength(300)]
        public string? ResumePath { get; set; }

        [StringLength(200)]
        public string? ResumeFileName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }
        public virtual ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}
