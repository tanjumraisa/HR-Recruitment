using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    public class Interview : ISoftDelete
    {
        [Key]
        public int InterviewId { get; set; }

        public int ApplicationId { get; set; }

        public int InterviewerId { get; set; }

        public DateTime ScheduledAt { get; set; }

        [Required, StringLength(30)]
        public string Mode { get; set; } = "Onsite";

        [StringLength(150)]
        public string? Location { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = InterviewStatus.Scheduled;

        public int? Score { get; set; }

        [StringLength(1000)]
        public string? Feedback { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(ApplicationId))]
        public virtual JobApplication Application { get; set; } = null!;
        [ForeignKey(nameof(InterviewerId))]
        public virtual Employee Interviewer { get; set; } = null!;
    }
}
