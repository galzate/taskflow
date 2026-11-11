using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs.Tasks;
using TaskFlow.Api.Enums;
using TaskFlow.Api.Services;

namespace TaskFlow.Api.Controllers;

/// <summary>
/// REST endpoints for task management.
/// 10-11-2026 Construido por: Ing GAAE - Tasks API controller (create, list, filter and change status).
/// </summary>
[ApiController]
[Route("api/tasks")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Returns tasks optionally filtered by user and status, sorted by creation date.
    /// Use sort=asc for ascending order (default is descending).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TaskResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TaskResponse>>> GetAll(
        [FromQuery] int? userId,
        [FromQuery] TaskItemStatus? status,
        [FromQuery] string? sort,
        CancellationToken cancellationToken)
    {
        var tasks = await _taskService.GetAsync(userId, status, sort, cancellationToken);
        return Ok(tasks);
    }

    /// <summary>Returns a single task by its identifier.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var task = await _taskService.GetByIdAsync(id, cancellationToken);
        return Ok(task);
    }

    /// <summary>Creates a new task assigned to an existing user.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> Create(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var createdTask = await _taskService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createdTask);
    }

    /// <summary>Changes the status of a task following the allowed transitions.</summary>
    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<TaskResponse>> UpdateStatus(
        int id,
        [FromBody] UpdateTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        var updatedTask = await _taskService.UpdateStatusAsync(id, request, cancellationToken);
        return Ok(updatedTask);
    }
}
