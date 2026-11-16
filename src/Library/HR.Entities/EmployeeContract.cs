using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Employment contract
    [DisplayColumn(nameof(ContractNo))]
    public class EmployeeContract : ISoftDelete
    {
        [Key]
        public int EmployeeContractId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Employment type"), Range(1, int.MaxValue, ErrorMessage = "Please select employment type.")]
        public int EmploymentTypeId { get; set; }

        [Display(Name = "Grade"), Range(1, int.MaxValue, ErrorMessage = "Please select grade.")]
        public int GradeId { get; set; }

        [Display(Name = "Contract no"), Required, StringLength(30)]
        public string ContractNo { get; set; } = string.Empty;

        [Display(Name = "Start date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Display(Name = "End date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Probation months"), Range(0, 24)]
        public int ProbationMonths { get; set; }

        [Display(Name = "Basic salary"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal BasicSalary { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Draft|Active|Expired|Terminated")]
        public string Status { get; set; } = "Active";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(EmploymentTypeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual EmploymentType EmploymentType { get; set; } = null!;

        [ForeignKey(nameof(GradeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Grade Grade { get; set; } = null!;
    }
}
