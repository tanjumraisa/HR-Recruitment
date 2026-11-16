using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Financial year (July-June)
    [DisplayColumn(nameof(Name))]
    public class FinancialYear : ISoftDelete
    {
        [Key]
        public int FinancialYearId { get; set; }

        [Display(Name = "Name"), Required, StringLength(20)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Start date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Display(Name = "End date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today;

        [Display(Name = "Current year")]
        public bool IsCurrent { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}
