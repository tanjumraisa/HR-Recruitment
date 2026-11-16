using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Every status change of an application
    public class ApplicationStatusHistory : ISoftDelete
    {
        [Key]
        public int ApplicationStatusHistoryId { get; set; }

        [Display(Name = "Application"), Range(1, int.MaxValue, ErrorMessage = "Please select application.")]
        public int ApplicationId { get; set; }

        [Display(Name = "From status"), StringLength(20)]
        public string? FromStatus { get; set; }

        [Display(Name = "To status"), Required, StringLength(20)]
        public string ToStatus { get; set; } = string.Empty;

        [Display(Name = "Changed by")]
        public int? ChangedById { get; set; }

        [Display(Name = "Changed at")]
        public DateTime ChangedAt { get; set; } = DateTime.Now;

        [Display(Name = "Remarks"), StringLength(250)]
        public string? Remarks { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(ApplicationId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual JobApplication Application { get; set; } = null!;

        [ForeignKey(nameof(ChangedById))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual User? ChangedBy { get; set; }
    }
}
