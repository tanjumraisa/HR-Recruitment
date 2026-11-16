using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    public class Designation : ISoftDelete
    {
        [Key]
        public int DesignationId { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal BaseSalary { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public virtual ICollection<JobOpening> JobOpenings { get; set; } = new List<JobOpening>();
    }
}
