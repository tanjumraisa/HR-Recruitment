using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    public class Employee : ISoftDelete
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required, StringLength(20)]
        public string EmployeeCode { get; set; } = string.Empty;

        public int UserId { get; set; }

        public int DepartmentId { get; set; }

        public int DesignationId { get; set; }

        public int? ManagerId { get; set; }

        // Workstation / office of the employee (optional).
        public int? BranchId { get; set; }

        [Column(TypeName = "date")]
        public DateTime JoinDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Salary { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = EmployeeStatus.Active;

        [StringLength(250)]
        public string? Address { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual User User { get; set; } = null!;
        [ForeignKey(nameof(DepartmentId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Department Department { get; set; } = null!;
        [ForeignKey(nameof(DesignationId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Designation Designation { get; set; } = null!;
        [ForeignKey(nameof(BranchId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Branch? Branch { get; set; }
        [ForeignKey(nameof(ManagerId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee? Manager { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<PerformanceReview> PerformanceReviews { get; set; } = new List<PerformanceReview>();
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
    }
}
