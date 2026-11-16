using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Who changed what and when (written automatically)
    public class AuditLog
    {
        [Key]
        public int AuditLogId { get; set; }

        [Display(Name = "Entity name"), Required, StringLength(100)]
        public string EntityName { get; set; } = string.Empty;

        [Display(Name = "Record"), StringLength(50)]
        public string? RecordId { get; set; }

        [Display(Name = "Action"), Required, StringLength(20)]
        public string Action { get; set; } = string.Empty;

        [Display(Name = "Changes"), StringLength(2000), DataType(DataType.MultilineText)]
        public string? Changes { get; set; }

        [Display(Name = "User name"), StringLength(150)]
        public string? UserName { get; set; }

        [Display(Name = "Created at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
