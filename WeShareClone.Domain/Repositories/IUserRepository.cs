using WeShareClone.Domain.Models;

namespace WeShareClone.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<User> CreateAsync(string email, string name);
    Task<User?> UpdateNameAsync(int id, string name);
}