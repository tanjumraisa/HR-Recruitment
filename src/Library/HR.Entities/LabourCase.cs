using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Case in labour court
    [DisplayColumn(nameof(CaseNo))]
    public class LabourCase : ISoftDelete
    {
        [Key]
        public int LabourCaseId { get; set; }

        [Display(Name = "Case no"), Required, StringLength(50)]
        public string CaseNo { get; set; } = string.Empty;

        [Display(Name = "Employee")]
        public int? EmployeeId { get; set; }

        [Display(Name = "Court"), StringLength(150)]
        public string? Court { get; set; }

        [Display(Name = "Filed on"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime FiledOn { get; set; } = DateTime.Today;

        [Display(Name = "Next hearing"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime? NextHearing { get; set; }

        [Display(Name = "Details"), StringLength(2000), DataType(DataType.MultilineText)]
        public string? Details { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Running|Settled|Closed")]
        public string Status { get; set; } = "Running";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee? Employee { get; set; }
    }
}
