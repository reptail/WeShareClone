using WeShareClone.Api.Domain.Models;

namespace WeShareClone.Api.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<User> CreateAsync(string email, string name);
}