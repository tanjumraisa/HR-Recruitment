using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Where candidates come from
    [DisplayColumn(nameof(Name))]
    public class RecruitmentSource : ISoftDelete
    {
        [Key]
        public int RecruitmentSourceId { get; set; }

        [Display(Name = "Name"), Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Source type"), Required, StringLength(20), Choices("JobPortal|Referral|SocialMedia|Agency|Campus|WalkIn")]
        public string SourceType { get; set; } = "JobPortal";

        [Display(Name = "Website"), StringLength(200)]
        public string? Website { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}
