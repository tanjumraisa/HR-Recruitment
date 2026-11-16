using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Employee in a team
    public class TeamMember : ISoftDelete
    {
        [Key]
        public int TeamMemberId { get; set; }

        [Display(Name = "Team"), Range(1, int.MaxValue, ErrorMessage = "Please select team.")]
        public int TeamId { get; set; }

        [Display(Name = "Employee"), Range(1, int.MaxValue, ErrorMessage = "Please select employee.")]
        public int EmployeeId { get; set; }

        [Display(Name = "Role in team"), Required, StringLength(30), Choices("Member|Lead|Coordinator")]
        public string MemberRole { get; set; } = "Member";

        [Display(Name = "Joined on"), Column(TypeName = "date"), DataType(DataType.Date)]
        public DateTime JoinedOn { get; set; } = DateTime.Today;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [ForeignKey(nameof(TeamId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Team Team { get; set; } = null!;

        [ForeignKey(nameof(EmployeeId))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Employee Employee { get; set; } = null!;
    }
}
