using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Entities;

namespace TaskFlow.Api.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IUserRepository"/>.
/// 10-11-2026 Construido por: Ing GAAE - Reads and writes users through the DbContext.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly TaskFlowDbContext _dbContext;

    public UserRepository(TaskFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.AnyAsync(user => user.Email == email, cancellationToken);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }
}
