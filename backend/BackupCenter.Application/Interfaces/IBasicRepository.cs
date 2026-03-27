using System.Collections.Generic;
using System.Threading.Tasks;
using BackupCenter.Domain.Entities;

namespace BackupCenter.Application.Interfaces;

/// Define operaciones CRUD genéricas para cualquier entidad.
/// Este repositorio abstrae el acceso a datos y permite
/// desacoplar la lógica de negocio del ORM.
public interface IBasicRepository<T> where T : class
{
    /// Obtiene todos los registros de la entidad.
    Task<IEnumerable<T>> GetAllAsync();

    /// Obtiene todos los registros de la entidad.
    Task<T?> GetByIdAsync(int id);

    /// Obtiene una entidad por su ID.
    Task AddAsync(T entity);

    /// Agrega una nueva entidad.
    Task UpdateAsync(T entity);

    /// Actualiza una entidad existente.
    Task DeleteAsync(int id);
}
