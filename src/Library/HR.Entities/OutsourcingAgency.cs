using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HR.Framework;

namespace HR.Entities
{
    // Manpower supplier company
    [DisplayColumn(nameof(Name))]
    public class OutsourcingAgency : ISoftDelete
    {
        [Key]
        public int OutsourcingAgencyId { get; set; }

        [Display(Name = "Name"), Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Contact person"), StringLength(100)]
        public string? ContactPerson { get; set; }

        [Display(Name = "Phone"), StringLength(20)]
        public string? Phone { get; set; }

        [Display(Name = "Address"), StringLength(250)]
        public string? Address { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}
