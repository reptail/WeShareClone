using WeShareClone.Domain.Models;

namespace WeShareClone.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<User[]> SearchAsync(string query);
    Task<User> CreateAsync(string email, string name, string? phone = null);
    Task<User?> UpdateNameAsync(int id, string name);
    Task<User?> UpdateProfileAsync(int id, string name, string? phone);
}