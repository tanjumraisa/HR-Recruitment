using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Request for new staff
    [DisplayColumn(nameof(Title))]
    public class ManpowerRequisition : ISoftDelete
    {
        [Key]
        public int ManpowerRequisitionId { get; set; }

        [Display(Name = "Title"), Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Department"), Range(1, int.MaxValue, ErrorMessage = "Please select department.")]
        public int DepartmentId { get; set; }

        [Display(Name = "Designation"), Range(1, int.MaxValue, ErrorMessage = "Please select designation.")]
        public int DesignationId { get; set; }

        [Display(Name = "Job category")]
        public int? JobCategoryId { get; set; }

        [Display(Name = "Requested by"), Range(1, int.MaxValue, ErrorMessage = "Please select requested by.")]
        public int RequestedById { get; set; }

        [Display(Name = "Number of positions"), Range(1, 500)]
        public int NumberOfPositions { get; set; } = 1;

        [Display(Name = "Required by"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime RequiredBy { get; set; } = DateTime.Today;

        [Display(Name = "Justification"), StringLength(2000), DataType(DataType.MultilineText)]
        public string? Justification { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Pending|Approved|Rejected|Fulfilled")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "Job opening created")]
        public int? JobOpeningId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Department Department { get; set; } = null!;

        [ForeignKey(nameof(DesignationId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Designation Designation { get; set; } = null!;

        [ForeignKey(nameof(JobCategoryId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual JobCategory? JobCategory { get; set; }

        [ForeignKey(nameof(RequestedById))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee RequestedBy { get; set; } = null!;

        [ForeignKey(nameof(JobOpeningId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual JobOpening? JobOpening { get; set; }
    }
}
