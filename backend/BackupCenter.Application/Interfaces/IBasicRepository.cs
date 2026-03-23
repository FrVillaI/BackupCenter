using System.Collections.Generic;
using System.Threading.Tasks;
using BackupCenter.Domain.Entities;

namespace BackupCenter.Application.Interfaces;

public interface IBasicRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
