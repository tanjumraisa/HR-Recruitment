using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    public class JobOpening : ISoftDelete
    {
        [Key]
        public int JobOpeningId { get; set; }

        [Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        public int DepartmentId { get; set; }

        public int DesignationId { get; set; }

        public int Vacancies { get; set; } = 1;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string? Requirements { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SalaryMin { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SalaryMax { get; set; }

        [Required, StringLength(30)]
        public string EmploymentType { get; set; } = "Full-time";

        [Column(TypeName = "date")]
        public DateTime Deadline { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = JobStatus.Draft;

        public int PostedById { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public virtual Department Department { get; set; } = null!;
        [ForeignKey(nameof(DesignationId))]
        public virtual Designation Designation { get; set; } = null!;
        [ForeignKey(nameof(PostedById))]
        public virtual User PostedBy { get; set; } = null!;
        public virtual ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}
