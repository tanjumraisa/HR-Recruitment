using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Health, safety and environment incident
    public class HseIncident : ISoftDelete
    {
        [Key]
        public int HseIncidentId { get; set; }

        [Display(Name = "Incident date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime IncidentDate { get; set; } = DateTime.Today;

        [Display(Name = "Workstation")]
        public int? BranchId { get; set; }

        [Display(Name = "Affected employee")]
        public int? EmployeeId { get; set; }

        [Display(Name = "Incident type"), Required, StringLength(20), Choices("Injury|NearMiss|Fire|Illness|Other")]
        public string IncidentType { get; set; } = "NearMiss";

        [Display(Name = "Severity"), Required, StringLength(20), Choices("Low|Medium|High")]
        public string Severity { get; set; } = "Low";

        [Display(Name = "Description"), Required, StringLength(2000), DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Action taken"), StringLength(2000), DataType(DataType.MultilineText)]
        public string? ActionTaken { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Open|Closed")]
        public string Status { get; set; } = "Open";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(BranchId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Branch? Branch { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee? Employee { get; set; }
    }
}
