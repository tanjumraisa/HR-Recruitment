using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    public class OfferLetter : ISoftDelete
    {
        [Key]
        public int OfferId { get; set; }

        public int ApplicationId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal OfferedSalary { get; set; }

        [Column(TypeName = "date")]
        public DateTime JoiningDate { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = OfferStatus.Issued;

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(ApplicationId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual JobApplication Application { get; set; } = null!;
    }
}
