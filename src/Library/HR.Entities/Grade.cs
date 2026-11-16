using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Pay grade with salary band
    [DisplayColumn(nameof(Name))]
    public class Grade : ISoftDelete
    {
        [Key]
        public int GradeId { get; set; }

        [Display(Name = "Name"), Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Level"), Range(1, 50)]
        public int Level { get; set; }

        [Display(Name = "Min salary"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal MinSalary { get; set; }

        [Display(Name = "Max salary"), Column(TypeName = "decimal(18, 2)"), Range(typeof(decimal), "0", "999999999")]
        public decimal MaxSalary { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}
