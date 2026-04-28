using System;
using System.Linq;
using System.Threading.Tasks;
using BdsAdmin.API.Entities;

namespace BdsAdmin.API.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task<Category?> GetByIdWithRelationsAsync(Guid id);
    Task AddAsync(Category category);
    void Remove(Category category);
    Task SaveChangesAsync();
    Task<bool> ExistsAsync(Guid id);
}
