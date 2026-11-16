using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Monthly wage of outsourced worker
    public class OutsourcedSalary : ISoftDelete
    {
        [Key]
        public int OutsourcedSalaryId { get; set; }

        [Display(Name = "Worker"), Range(1, int.MaxValue, ErrorMessage = "Please select worker.")]
        public int OutsourcedEmployeeId { get; set; }

        [Display(Name = "Month"), Range(1, 12)]
        public int Month { get; set; }

        [Display(Name = "Year"), Range(2000, 2100)]
        public int Year { get; set; }

        [Display(Name = "Present days (calculated)"), Range(0, 31)]
        public int WorkingDays { get; set; }

        [Display(Name = "Daily rate"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal DailyRate { get; set; }

        [Display(Name = "Amount (calculated)"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal Amount { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Generated|Paid")]
        public string Status { get; set; } = "Generated";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(OutsourcedEmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual OutsourcedEmployee OutsourcedEmployee { get; set; } = null!;
    }
}
