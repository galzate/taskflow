using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs.Users;
using TaskFlow.Api.Services;

namespace TaskFlow.Api.Controllers;

/// <summary>
/// REST endpoints for user management.
/// 10-11-2026 Construido por: Ing GAAE - Users API controller (create, list and get by id).
/// </summary>
[ApiController]
[Route("api/users")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>Returns every registered user.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    /// <summary>Returns a single user by its identifier.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        return Ok(user);
    }

    /// <summary>Creates a new user.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<UserResponse>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var createdUser = await _userService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
    }
}
