using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    public class Payroll : ISoftDelete
    {
        [Key]
        public int PayrollId { get; set; }

        public int EmployeeId { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal BasicSalary { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Allowance { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Deduction { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal NetSalary { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = PayrollStatus.Generated;

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;
    }
}
