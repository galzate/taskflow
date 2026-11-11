using System.Text.Json;
using TaskFlow.Api.DTOs.Tasks;
using TaskFlow.Api.Entities;
using TaskFlow.Api.Enums;
using TaskFlow.Api.Exceptions;
using TaskFlow.Api.Repositories;
using TaskFlow.Api.Services;

namespace TaskFlow.Api.Tests;

// 10-11-2026 Construido por: Ing GAAE - Unit tests for the task business rules.
public class TaskServiceTests
{
    private static readonly User ExistingUser = new()
    {
        Id = 1,
        Name = "Ana Torres",
        Email = "ana.torres@taskflow.dev"
    };

    [Fact]
    public async Task UpdateStatusAsync_WhenMovingFromPendingToDone_ThrowsBusinessRuleException()
    {
        var taskRepository = new FakeTaskRepository();
        var task = await taskRepository.AddAsync(new TaskItem
        {
            Title = "Sample task",
            AssignedUserId = ExistingUser.Id,
            Status = TaskItemStatus.Pending
        });
        var service = CreateService(taskRepository);

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            service.UpdateStatusAsync(task.Id, new UpdateTaskStatusRequest { Status = TaskItemStatus.Done }));

        Assert.Contains("Pending to Done", exception.Message);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenMovingFromPendingToInProgress_UpdatesTheStatus()
    {
        var taskRepository = new FakeTaskRepository();
        var task = await taskRepository.AddAsync(new TaskItem
        {
            Title = "Sample task",
            AssignedUserId = ExistingUser.Id,
            Status = TaskItemStatus.Pending
        });
        var service = CreateService(taskRepository);

        var updated = await service.UpdateStatusAsync(
            task.Id,
            new UpdateTaskStatusRequest { Status = TaskItemStatus.InProgress });

        Assert.Equal(TaskItemStatus.InProgress, updated.Status);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenMovingFromInProgressToDone_UpdatesTheStatus()
    {
        var taskRepository = new FakeTaskRepository();
        var task = await taskRepository.AddAsync(new TaskItem
        {
            Title = "Sample task",
            AssignedUserId = ExistingUser.Id,
            Status = TaskItemStatus.InProgress
        });
        var service = CreateService(taskRepository);

        var updated = await service.UpdateStatusAsync(
            task.Id,
            new UpdateTaskStatusRequest { Status = TaskItemStatus.Done });

        Assert.Equal(TaskItemStatus.Done, updated.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenStatusIsNotDefined_ThrowsValidationException()
    {
        var service = CreateService(new FakeTaskRepository());

        await Assert.ThrowsAsync<ValidationException>(() =>
            service.UpdateStatusAsync(1, new UpdateTaskStatusRequest { Status = (TaskItemStatus)99 }));
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenTaskDoesNotExist_ThrowsNotFoundException()
    {
        var service = CreateService(new FakeTaskRepository());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.UpdateStatusAsync(999, new UpdateTaskStatusRequest { Status = TaskItemStatus.InProgress }));
    }

    [Fact]
    public async Task CreateAsync_WhenUserDoesNotExist_ThrowsNotFoundException()
    {
        var service = new TaskService(new FakeTaskRepository(), new FakeUserRepository());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.CreateAsync(new CreateTaskRequest
            {
                Title = "Sample task",
                AssignedUserId = 999
            }));
    }

    [Fact]
    public async Task CreateAsync_WhenMetadataIsNotAnObject_ThrowsValidationException()
    {
        using var document = JsonDocument.Parse("[1, 2, 3]");
        var service = CreateService(new FakeTaskRepository());

        await Assert.ThrowsAsync<ValidationException>(() =>
            service.CreateAsync(new CreateTaskRequest
            {
                Title = "Sample task",
                AssignedUserId = ExistingUser.Id,
                Metadata = document.RootElement.Clone()
            }));
    }

    [Fact]
    public async Task CreateAsync_WithValidData_StartsAsPending()
    {
        var service = CreateService(new FakeTaskRepository());
        using var document = JsonDocument.Parse("""{"priority":"High"}""");

        var created = await service.CreateAsync(new CreateTaskRequest
        {
            Title = "  Sample task  ",
            AssignedUserId = ExistingUser.Id,
            Metadata = document.RootElement.Clone()
        });

        Assert.Equal("Sample task", created.Title);
        Assert.Equal(TaskItemStatus.Pending, created.Status);
        Assert.Equal(ExistingUser.Name, created.AssignedUserName);
        Assert.NotNull(created.Metadata);
    }

    private static TaskService CreateService(FakeTaskRepository taskRepository)
    {
        return new TaskService(taskRepository, new FakeUserRepository(ExistingUser));
    }

    /// <summary>
    /// In-memory user repository used to isolate the business rules from the database.
    /// </summary>
    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public FakeUserRepository(params User[] users)
        {
            _users = users.ToList();
        }

        public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<User>>(_users);

        public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.FirstOrDefault(user => user.Id == id));

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.Any(user => user.Email == email));

        public Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            user.Id = _users.Count + 1;
            _users.Add(user);
            return Task.FromResult(user);
        }
    }

    /// <summary>
    /// In-memory task repository used to isolate the business rules from the database.
    /// </summary>
    private sealed class FakeTaskRepository : ITaskRepository
    {
        private readonly List<TaskItem> _tasks = new();

        public Task<IReadOnlyList<TaskItem>> GetAsync(
            int? userId,
            TaskItemStatus? status,
            bool ascending,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskItem> query = _tasks;

            if (userId.HasValue)
            {
                query = query.Where(task => task.AssignedUserId == userId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(task => task.Status == status.Value);
            }

            query = ascending
                ? query.OrderBy(task => task.CreatedAt)
                : query.OrderByDescending(task => task.CreatedAt);

            return Task.FromResult<IReadOnlyList<TaskItem>>(query.ToList());
        }

        public Task<TaskItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(_tasks.FirstOrDefault(task => task.Id == id));

        public Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            task.Id = _tasks.Count + 1;
            _tasks.Add(task);
            return Task.FromResult(task);
        }

        public Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
