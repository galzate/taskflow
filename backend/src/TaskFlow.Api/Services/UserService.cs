using TaskFlow.Api.DTOs.Users;
using TaskFlow.Api.Entities;
using TaskFlow.Api.Exceptions;
using TaskFlow.Api.Repositories;

namespace TaskFlow.Api.Services;

/// <summary>
/// Business logic for user management.
/// 10-11-2026 Construido por: Ing GAAE - Validates unique emails and maps entities to response DTOs.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return users.Select(MapToResponse).ToList();
    }

    public async Task<UserResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException($"No se encontró el usuario con id {id}.");
        }

        return MapToResponse(user);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        // Emails are stored normalized (trimmed and lower-cased) to keep uniqueness consistent.
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (await _userRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
        {
            throw new BusinessRuleException($"Ya existe un usuario con el correo electrónico '{normalizedEmail}'.");
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = normalizedEmail,
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.AddAsync(user, cancellationToken);
        return MapToResponse(createdUser);
    }

    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }
}
