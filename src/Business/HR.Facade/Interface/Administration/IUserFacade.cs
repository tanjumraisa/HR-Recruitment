using HR.Entities;

namespace HR.Facade.Interface
{
    public interface IUserFacade
    {
        Task<List<User>> GetUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task AddUserAsync(User entity);
        Task<bool> UpdateUserAsync(User entity);
        Task<bool> DeleteUserAsync(int id);
        Task<User> RegisterCandidateAsync(string fullName, string email, string? phone, string password);
        Task<User?> AuthenticateAsync(string email, string password);
        Task EnsureAdminAsync(string fullName, string email, string password);
    }
}
