using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Provident, gratuity, welfare ... fund
    [DisplayColumn(nameof(Name))]
    public class Fund : ISoftDelete
    {
        [Key]
        public int FundId { get; set; }

        [Display(Name = "Name"), Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Fund type"), Required, StringLength(30), Choices("ProvidentFund|Gratuity|SocialSecurity|WorkersParticipation|WorkersWelfare")]
        public string FundType { get; set; } = "ProvidentFund";

        [Display(Name = "Employee contribution %"), Column(TypeName = "decimal(6, 2)"), Range(typeof(decimal), "0", "100")]
        public decimal EmployeeContributionPercent { get; set; }

        [Display(Name = "Employer contribution %"), Column(TypeName = "decimal(6, 2)"), Range(typeof(decimal), "0", "100")]
        public decimal EmployerContributionPercent { get; set; }

        [Display(Name = "Description"), StringLength(250)]
        public string? Description { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}
