using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Job opening published on a source
    public class JobAdvertisement : ISoftDelete
    {
        [Key]
        public int JobAdvertisementId { get; set; }

        [Display(Name = "Job opening"), Range(1, int.MaxValue, ErrorMessage = "Please select job opening.")]
        public int JobOpeningId { get; set; }

        [Display(Name = "Recruitment source"), Range(1, int.MaxValue, ErrorMessage = "Please select recruitment source.")]
        public int RecruitmentSourceId { get; set; }

        [Display(Name = "Posted on"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime PostedOn { get; set; } = DateTime.Today;

        [Display(Name = "Expires on"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime? ExpiresOn { get; set; }

        [Display(Name = "Cost"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal Cost { get; set; }

        [Display(Name = "Url"), StringLength(300)]
        public string? Url { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(JobOpeningId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual JobOpening JobOpening { get; set; } = null!;

        [ForeignKey(nameof(RecruitmentSourceId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual RecruitmentSource RecruitmentSource { get; set; } = null!;
    }
}
