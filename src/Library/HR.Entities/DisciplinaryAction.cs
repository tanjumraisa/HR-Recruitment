using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Warning, show cause, suspension
    public class DisciplinaryAction : ISoftDelete
    {
        [Key]
        public int DisciplinaryActionId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Issued by")]
        public int? IssuedById { get; set; }

        [Display(Name = "Incident date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime IncidentDate { get; set; } = DateTime.Today;

        [Display(Name = "Action type"), Required, StringLength(30), Choices("VerbalWarning|WrittenWarning|ShowCause|Suspension|Termination")]
        public string ActionType { get; set; } = "VerbalWarning";

        [Display(Name = "Description"), Required, StringLength(2000), DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Status"), Required, StringLength(20), Choices("Open|Closed")]
        public string Status { get; set; } = "Open";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(IssuedById))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee? IssuedBy { get; set; }
    }
}
