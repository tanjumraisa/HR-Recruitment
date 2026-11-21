using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HR.Entities
{
    // One permission = one thing a user may do, e.g. "Employee.Create".
    // ParentId builds the tree: Menu (Employee Information) -> Screen (Employee) -> Action (Create).
    [DisplayColumn(nameof(DisplayText))]
    public class Permission
    {
        // Not an identity column (same as the faculty SQL): ids are given by the permission catalog.
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PermissionId { get; set; }

        public int CompanyId { get; set; } = 1;

        public int? ParentId { get; set; }

        [Required, StringLength(500)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? DisplayText { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(CompanyId)), JsonIgnore] public virtual Company Company { get; set; } = null!;
        [ForeignKey(nameof(ParentId)), JsonIgnore] public virtual Permission? Parent { get; set; }
        [JsonIgnore] public virtual ICollection<Permission> Children { get; set; } = new List<Permission>();
        [JsonIgnore] public virtual ICollection<PermissionGroupMap> GroupMaps { get; set; } = new List<PermissionGroupMap>();
        [JsonIgnore] public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}
