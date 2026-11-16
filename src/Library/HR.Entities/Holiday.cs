using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Holiday calendar
    [DisplayColumn(nameof(Name))]
    public class Holiday : ISoftDelete
    {
        [Key]
        public int HolidayId { get; set; }

        [Display(Name = "Name"), Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Date"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Display(Name = "Holiday type"), Required, StringLength(20), Choices("Public|Religious|Company")]
        public string HolidayType { get; set; } = "Public";

        [Display(Name = "Branch (empty = all)")]
        public int? BranchId { get; set; }

        [Display(Name = "Remarks"), StringLength(250)]
        public string? Remarks { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(BranchId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Branch? Branch { get; set; }
    }
}
