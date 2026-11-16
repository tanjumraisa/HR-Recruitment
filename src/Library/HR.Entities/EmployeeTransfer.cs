using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Department / workstation transfer
    public class EmployeeTransfer : ISoftDelete
    {
        [Key]
        public int EmployeeTransferId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "From department")]
        public int? FromDepartmentId { get; set; }

        [Display(Name = "To department"), Range(1, int.MaxValue, ErrorMessage = "Please select to department.")]
        public int ToDepartmentId { get; set; }

        [Display(Name = "From workstation")]
        public int? FromBranchId { get; set; }

        [Display(Name = "To workstation")]
        public int? ToBranchId { get; set; }

        [Display(Name = "Transfer date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime TransferDate { get; set; } = DateTime.Today;

        [Display(Name = "Reason"), StringLength(500)]
        public string? Reason { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Pending|Approved|Rejected")]
        public string Status { get; set; } = "Pending";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(FromDepartmentId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Department? FromDepartment { get; set; }

        [ForeignKey(nameof(ToDepartmentId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Department ToDepartment { get; set; } = null!;

        [ForeignKey(nameof(FromBranchId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Branch? FromBranch { get; set; }

        [ForeignKey(nameof(ToBranchId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Branch? ToBranch { get; set; }
    }
}
