using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Internal HR audit
    [DisplayColumn(nameof(Title))]
    public class HrAudit : ISoftDelete
    {
        [Key]
        public int HrAuditId { get; set; }

        [Display(Name = "Title"), Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }

        [Display(Name = "Audit date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime AuditDate { get; set; } = DateTime.Today;

        [Display(Name = "Auditor"), StringLength(100)]
        public string? Auditor { get; set; }

        [Display(Name = "Score"), Range(0, 100)]
        public int Score { get; set; }

        [Display(Name = "Findings"), StringLength(2000), DataType(DataType.MultilineText)]
        public string? Findings { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Planned|Done")]
        public string Status { get; set; } = "Planned";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Department? Department { get; set; }
    }
}
