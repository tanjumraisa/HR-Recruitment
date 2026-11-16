using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    public class PerformanceReview : ISoftDelete
    {
        [Key]
        public int ReviewId { get; set; }

        public int EmployeeId { get; set; }

        public int ReviewerId { get; set; }

        [Required, StringLength(30)]
        public string Period { get; set; } = string.Empty;

        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comments { get; set; }

        public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; } = null!;
        [ForeignKey(nameof(ReviewerId))]
        public virtual Employee Reviewer { get; set; } = null!;
    }
}
