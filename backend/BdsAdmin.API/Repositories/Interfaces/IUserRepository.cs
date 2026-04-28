using System;
using System.Linq;
using System.Threading.Tasks;
using BdsAdmin.API.Entities;

namespace BdsAdmin.API.Repositories.Interfaces;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByIdWithRelationsAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task AddAsync(User user);
    void Remove(User user);
    Task SaveChangesAsync();
    Task<bool> ExistsAsync(Guid id);
}
