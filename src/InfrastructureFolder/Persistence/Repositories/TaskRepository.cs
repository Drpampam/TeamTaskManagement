using Application.Interfaces;
using Application.Interfaces.Persistence;
using Application.Responses;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Task = Domain.Models.Task;

namespace Persistence.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IAsyncRepository<Task> _taskRepository;
        private readonly IAsyncRepository<TeamUser> _teamUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TaskRepository> _logger;

        public TaskRepository(
            IUnitOfWork unitOfWork,
            IAsyncRepository<TeamUser> teamUserRepository,
            IAsyncRepository<Task> taskRepository,
            ILogger<TaskRepository> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _teamUserRepository = teamUserRepository ?? throw new ArgumentNullException(nameof(teamUserRepository));
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BaseResponse<List<TaskDto>>> GetTeamTasks(GetTaskDto dto)
        {
            _logger.LogInformation("Retrieving tasks for team: {TeamId}", dto?.TeamId);

            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.TeamId))
                {
                    _logger.LogWarning("GetTeamTasks attempt with invalid DTO or TeamId");
                    return new BaseResponse<List<TaskDto>>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Invalid team ID."
                    };
                }

                var tasks = await _taskRepository
                    .WhereQueryable(t => t.TeamId == dto.TeamId)
                    .Result
                    .Include(t => t.AssignedToUser)
                    .Select(t => new TaskDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        DueDate = t.DueDate,
                        Status = t.Status.ToString(),
                        CreatedAt = t.CreatedAt,
                        AssignedToUserId = t.AssignedToUserId ?? string.Empty,
                        AssignedToUsername = t.AssignedToUser != null ? t.AssignedToUser.Username : string.Empty,
                        CreatedByUserId = t.CreatedByUserId,
                        TeamId = t.TeamId
                    })
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {TaskCount} tasks for team: {TeamId}", tasks.Count, dto.TeamId);
                return new BaseResponse<List<TaskDto>>
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Message = "Tasks retrieved successfully",
                    Data = tasks
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve tasks for team: {TeamId}", dto?.TeamId);
                return new BaseResponse<List<TaskDto>>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while retrieving team tasks."
                };
            }
        }
    }
}