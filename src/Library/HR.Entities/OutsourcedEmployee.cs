using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Contract / outsourced worker
    [DisplayColumn(nameof(FullName))]
    public class OutsourcedEmployee : ISoftDelete
    {
        [Key]
        public int OutsourcedEmployeeId { get; set; }

        [Display(Name = "Full name"), Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Agency"), Range(1, int.MaxValue, ErrorMessage = "Please select agency.")]
        public int OutsourcingAgencyId { get; set; }

        [Display(Name = "Department"), Range(1, int.MaxValue, ErrorMessage = "Please select department.")]
        public int DepartmentId { get; set; }

        [Display(Name = "Workstation")]
        public int? BranchId { get; set; }

        [Display(Name = "Job title"), StringLength(100)]
        public string? JobTitle { get; set; }

        [Display(Name = "Phone"), StringLength(20)]
        public string? Phone { get; set; }

        [Display(Name = "NID"), StringLength(20)]
        public string? NationalId { get; set; }

        [Display(Name = "Join date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime JoinDate { get; set; } = DateTime.Today;

        [Display(Name = "Daily rate"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal DailyRate { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Active|Inactive")]
        public string Status { get; set; } = "Active";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(OutsourcingAgencyId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual OutsourcingAgency OutsourcingAgency { get; set; } = null!;

        [ForeignKey(nameof(DepartmentId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Department Department { get; set; } = null!;

        [ForeignKey(nameof(BranchId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Branch? Branch { get; set; }
    }
}
