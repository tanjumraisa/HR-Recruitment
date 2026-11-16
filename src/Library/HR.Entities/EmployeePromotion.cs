using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Promotion / increment
    public class EmployeePromotion : ISoftDelete
    {
        [Key]
        public int EmployeePromotionId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "From designation")]
        public int? FromDesignationId { get; set; }

        [Display(Name = "To designation"), Range(1, int.MaxValue, ErrorMessage = "Please select to designation.")]
        public int ToDesignationId { get; set; }

        [Display(Name = "Old salary"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal OldSalary { get; set; }

        [Display(Name = "New salary"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal NewSalary { get; set; }

        [Display(Name = "Effective date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; } = DateTime.Today;

        [Display(Name = "Promotion type"), Required, StringLength(20), Choices("Promotion|Increment|Demotion")]
        public string PromotionType { get; set; } = "Promotion";

        [Display(Name = "Remarks"), StringLength(250)]
        public string? Remarks { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Pending|Approved|Rejected")]
        public string Status { get; set; } = "Pending";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(FromDesignationId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Designation? FromDesignation { get; set; }

        [ForeignKey(nameof(ToDesignationId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Designation ToDesignation { get; set; } = null!;
    }
}
