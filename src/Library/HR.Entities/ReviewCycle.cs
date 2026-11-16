using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Appraisal period
    [DisplayColumn(nameof(Name))]
    public class ReviewCycle : ISoftDelete
    {
        [Key]
        public int ReviewCycleId { get; set; }

        [Display(Name = "Name"), Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Start date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Display(Name = "End date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today;

        [Display(Name = "Status"), Required, StringLength(20), Choices("Planned|Open|Closed")]
        public string Status { get; set; } = "Planned";

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}
