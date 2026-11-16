using HR.DataAccess.Interface;
using HR.Entities;
using HR.Facade.Interface;
using HR.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace HR.Facade.Implementation
{
    public class UserFacade : IUserFacade
    {
        private readonly IUserRepository _repo;
        private readonly ICandidateRepository _candidateRepo;
        private readonly PasswordHasher<User> _hasher = new();

        public UserFacade(IUserRepository repo, ICandidateRepository candidateRepo)
        {
            _repo = repo;
            _candidateRepo = candidateRepo;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddUserAsync(User entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            await ValidateAsync(e, null);
            e.PasswordHash = _hasher.HashPassword(e, e.PasswordHash); // PasswordHash holds the plain password only until here
            await _repo.AddAsync(e);
        }

        public async Task<bool> UpdateUserAsync(User entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var e = entity;
            var existing = await _repo.GetByIdAsync(e.UserId);
            if (existing == null) return false;

            await ValidateAsync(e, existing);
            e.PasswordHash = existing.PasswordHash; e.CreatedAt = existing.CreatedAt;
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            try
            {
                return await _repo.DeleteAsync(id);
            }
            catch (DbUpdateException)
            {
                throw new BusinessException("This record is used by other data and cannot be deleted.");
            }
        }

        private async Task ValidateAsync(User e, User? existing)
        {
            if (string.IsNullOrWhiteSpace(e.FullName)) throw new BusinessException("Full name is required.");
            if (string.IsNullOrWhiteSpace(e.Email) || !e.Email.Contains('@')) throw new BusinessException("A valid email is required.");
            e.Email = e.Email.Trim().ToLowerInvariant();
            var found = await _repo.GetByEmailAsync(e.Email);
            if (found != null && found.UserId != e.UserId) throw new BusinessException("This email is already registered.");
            if (existing == null && (string.IsNullOrEmpty(e.PasswordHash) || e.PasswordHash.Length < 8))
                throw new BusinessException("Password must be at least 8 characters.");
        }

        public async Task<User> RegisterCandidateAsync(string fullName, string email, string? phone, string password)
        {
            var user = new User
            {
                FullName = fullName,
                Email = email,
                PhoneNumber = phone,
                PasswordHash = password,
                RoleId = RoleIds.Candidate
            };
            await AddUserAsync(user);

            await _candidateRepo.AddAsync(new Candidate
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.PhoneNumber
            });
            return user;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _repo.GetByEmailAsync((email ?? string.Empty).Trim().ToLowerInvariant());
            if (user == null || !user.IsActive) return null;

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password ?? string.Empty);
            return result == PasswordVerificationResult.Failed ? null : user;
        }

        public async Task EnsureAdminAsync(string fullName, string email, string password)
        {
            var existingAdmin = await _repo.GetByEmailAsync(email.Trim().ToLowerInvariant());
            if (existingAdmin != null) return;

            await AddUserAsync(new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = password,
                RoleId = RoleIds.Admin
            });
        }
    }
}
