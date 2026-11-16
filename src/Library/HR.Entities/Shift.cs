using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Working shift
    [DisplayColumn(nameof(Name))]
    public class Shift : ISoftDelete
    {
        [Key]
        public int ShiftId { get; set; }

        [Display(Name = "Name"), Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Start time"), DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "End time"), DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "Grace minutes"), Range(0, 120)]
        public int GraceMinutes { get; set; }

        [Display(Name = "Night shift")]
        public bool IsNightShift { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}
