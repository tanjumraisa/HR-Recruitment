using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Employee member of a fund
    public class FundMembership : ISoftDelete
    {
        [Key]
        public int FundMembershipId { get; set; }

        [Display(Name = "Fund"), Range(1, int.MaxValue, ErrorMessage = "Please select fund.")]
        public int FundId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Join date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime JoinDate { get; set; } = DateTime.Today;

        [Display(Name = "Nominee")]
        public int? NomineeId { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Active|Closed")]
        public string Status { get; set; } = "Active";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(FundId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Fund Fund { get; set; } = null!;

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(NomineeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Dependent? Nominee { get; set; }
    }
}
