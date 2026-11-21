using HR.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess
{
    // User permission tables (faculty design): Permission, PermissionGroup, PermissionGroupMap, UserPermission + Company.
    public partial class ApplicationDbContext
    {
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<PermissionGroup> PermissionGroups => Set<PermissionGroup>();
        public DbSet<PermissionGroupMap> PermissionGroupMaps => Set<PermissionGroupMap>();
        public DbSet<UserPermission> UserPermissions => Set<UserPermission>();

        private static void ConfigurePermissions(ModelBuilder b)
        {
            b.Entity<Company>().HasData(new Company { CompanyId = 1, Name = "My Company", IsActive = true });

            b.Entity<Permission>().HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();
            b.Entity<Permission>().HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId);
            b.Entity<Permission>().HasOne(x => x.Company).WithMany(c => c.Permissions).HasForeignKey(x => x.CompanyId);

            b.Entity<PermissionGroup>().HasIndex(x => new { x.CompanyId, x.Name }).IsUnique();
            b.Entity<PermissionGroup>().HasOne(x => x.Company).WithMany(c => c.PermissionGroups).HasForeignKey(x => x.CompanyId);

            b.Entity<PermissionGroupMap>().HasIndex(x => new { x.PermissionGroupId, x.PermissionId }).IsUnique();
            b.Entity<PermissionGroupMap>().HasOne(x => x.PermissionGroup).WithMany(g => g.Maps).HasForeignKey(x => x.PermissionGroupId);
            b.Entity<PermissionGroupMap>().HasOne(x => x.Permission).WithMany(p => p.GroupMaps).HasForeignKey(x => x.PermissionId);
            b.Entity<PermissionGroupMap>().HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);

            b.Entity<UserPermission>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            b.Entity<UserPermission>().HasOne(x => x.Permission).WithMany(p => p.UserPermissions).HasForeignKey(x => x.PermissionId);
            b.Entity<UserPermission>().HasOne(x => x.PermissionGroup).WithMany(g => g.UserPermissions).HasForeignKey(x => x.PermissionGroupId);
            b.Entity<UserPermission>().HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
            // A row is either a group or a single permission, never empty.
            b.Entity<UserPermission>().ToTable(t => t.HasCheckConstraint("CK_UserPermission_GroupOrPermission",
                "[PermissionId] IS NOT NULL OR [PermissionGroupId] IS NOT NULL"));
        }
    }
}
