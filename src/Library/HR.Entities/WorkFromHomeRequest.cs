using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Remote work request
    public class WorkFromHomeRequest : ISoftDelete
    {
        [Key]
        public int WorkFromHomeRequestId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "From date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime FromDate { get; set; } = DateTime.Today;

        [Display(Name = "To date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime ToDate { get; set; } = DateTime.Today;

        [Display(Name = "Reason"), Required, StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "Status"), Required, StringLength(20), Choices("Pending|Approved|Rejected")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "Approved by")]
        public int? ApprovedById { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(ApprovedById))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee? ApprovedBy { get; set; }
    }
}
