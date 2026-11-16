using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Office / workstation of the company
    [DisplayColumn(nameof(Name))]
    public class Branch : ISoftDelete
    {
        [Key]
        public int BranchId { get; set; }

        [Display(Name = "Name"), Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Code"), StringLength(20)]
        public string? Code { get; set; }

        [Display(Name = "Address"), StringLength(250)]
        public string? Address { get; set; }

        [Display(Name = "Phone"), StringLength(20)]
        public string? Phone { get; set; }

        [Display(Name = "Head office")]
        public bool IsHeadOffice { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}
