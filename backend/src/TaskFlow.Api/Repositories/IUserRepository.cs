using TaskFlow.Api.Entities;

namespace TaskFlow.Api.Repositories;

/// <summary>
/// Data access contract for users. Implemented by <see cref="UserRepository"/>.
/// 10-11-2026 Construido por: Ing GAAE - Repository abstraction for the Users table.
/// </summary>
public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
}
