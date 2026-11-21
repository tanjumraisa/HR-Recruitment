using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HR.Entities
{
    // A named package of permissions, e.g. "Leave Officer".
    [DisplayColumn(nameof(Name))]
    public class PermissionGroup
    {
        [Key]
        public int PermissionGroupId { get; set; }

        public int CompanyId { get; set; } = 1;

        [Display(Name = "Group name"), Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Tag / note"), StringLength(500)]
        public string? Tag { get; set; }

        [ForeignKey(nameof(CompanyId)), JsonIgnore] public virtual Company Company { get; set; } = null!;
        [JsonIgnore] public virtual ICollection<PermissionGroupMap> Maps { get; set; } = new List<PermissionGroupMap>();
        [JsonIgnore] public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}
