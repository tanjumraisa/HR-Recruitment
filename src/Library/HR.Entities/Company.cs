using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HR.Entities
{
    // The company that uses the software. Today there is one company (CompanyId = 1),
    // but every permission table keeps CompanyId so more companies can be added later.
    [DisplayColumn(nameof(Name))]
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }

        [Display(Name = "Company name"), Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Address"), StringLength(250)]
        public string? Address { get; set; }

        [Display(Name = "Phone"), StringLength(20)]
        public string? Phone { get; set; }

        [Display(Name = "Email"), StringLength(150)]
        public string? Email { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [JsonIgnore] public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
        [JsonIgnore] public virtual ICollection<PermissionGroup> PermissionGroups { get; set; } = new List<PermissionGroup>();
    }
}
