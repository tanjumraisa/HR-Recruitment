using HR.Entities;

namespace HR.Facade.Interface
{
    // One entry of the permission catalog (built from the menu by the web project).
    public record PermissionDef(string Name, string DisplayText, string? ParentName);

    public record PermissionGroupSummary(int PermissionGroupId, string Name, string? Tag, int Permissions, int Users);

    public interface IPermissionFacade
    {
        // Adds missing permissions (and the default groups the first time). Safe to call at every start.
        Task SyncAsync(IReadOnlyList<PermissionDef> catalog);

        // null = the user has no group / permission assigned, so only the role decides.
        Task<HashSet<string>?> GetUserPermissionNamesAsync(int userId);

        Task<List<Permission>> GetAllPermissionsAsync();
        Task<List<PermissionGroupSummary>> GetGroupsAsync();
        Task<(PermissionGroup Group, HashSet<int> PermissionIds)> GetGroupAsync(int id);
        Task<int> SaveGroupAsync(int id, string name, string? tag, IEnumerable<int> permissionIds);
        Task DeleteGroupAsync(int id);

        Task<(HashSet<int> GroupIds, HashSet<int> PermissionIds)> GetUserAssignmentAsync(int userId);
        Task SaveUserAssignmentAsync(int userId, IEnumerable<int> groupIds, IEnumerable<int> permissionIds);
    }
}
