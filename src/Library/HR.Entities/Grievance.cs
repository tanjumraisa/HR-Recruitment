using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Employee complaint
    [DisplayColumn(nameof(Subject))]
    public class Grievance : ISoftDelete
    {
        [Key]
        public int GrievanceId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Subject"), Required, StringLength(150)]
        public string Subject { get; set; } = string.Empty;

        [Display(Name = "Details"), Required, StringLength(2000), DataType(DataType.MultilineText)]
        public string Details { get; set; } = string.Empty;

        [Display(Name = "Submitted on"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime SubmittedOn { get; set; } = DateTime.Today;

        [Display(Name = "Assigned to")]
        public int? AssignedToId { get; set; }

        [Display(Name = "Status"), Required, StringLength(20), Choices("Open|InReview|Resolved|Closed")]
        public string Status { get; set; } = "Open";

        [Display(Name = "Resolution"), StringLength(2000), DataType(DataType.MultilineText)]
        public string? Resolution { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(AssignedToId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee? AssignedTo { get; set; }
    }
}
