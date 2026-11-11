using TaskFlow.Api.DTOs.Users;

namespace TaskFlow.Api.Services;

/// <summary>
/// Business operations for users. Implemented by <see cref="UserService"/>.
/// 10-11-2026 Construido por: Ing GAAE - User service abstraction.
/// </summary>
public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<UserResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
}
