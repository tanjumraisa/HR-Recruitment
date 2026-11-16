using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Employee in a training
    public class TrainingEnrollment : ISoftDelete
    {
        [Key]
        public int TrainingEnrollmentId { get; set; }

        [Display(Name = "Training"), Range(1, int.MaxValue, ErrorMessage = "Please select training.")]
        public int TrainingProgramId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Enrolled|Completed|Cancelled")]
        public string Status { get; set; } = "Enrolled";

        [Display(Name = "Score"), Range(0, 100)]
        public int? Score { get; set; }

        [Display(Name = "Certificate no"), StringLength(50)]
        public string? CertificateNo { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(TrainingProgramId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual TrainingProgram TrainingProgram { get; set; } = null!;

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;
    }
}
