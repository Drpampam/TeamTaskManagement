using Application.Interfaces;
using Application.Interfaces.Persistence;
using Application.Responses;
using Domain.Common;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = Domain.Models.Task;

namespace Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly IAsyncRepository<Task> _taskRepository;
        private readonly IAsyncRepository<TeamUser> _teamUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITaskRepository _taskRepo;
        private readonly ILogger<TaskService> _logger;

        public TaskService(
            IUnitOfWork unitOfWork,
            IAsyncRepository<Task> repository,
            IAsyncRepository<TeamUser> teamUserRepository,
            ITaskRepository taskRepo,
            ILogger<TaskService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _taskRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _teamUserRepository = teamUserRepository ?? throw new ArgumentNullException(nameof(teamUserRepository));
            _taskRepo = taskRepo ?? throw new ArgumentNullException(nameof(taskRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BaseResponse<List<TaskDto>>> GetTasksAsync(GetTaskDto dto)
        {
            _logger.LogInformation("Retrieving tasks for team: {TeamId}, user: {UserId}", dto?.TeamId, dto?.UserId);

            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.TeamId) || string.IsNullOrEmpty(dto.UserId))
                {
                    _logger.LogWarning("GetTasksAsync attempt with invalid DTO, TeamId: {TeamId}, UserId: {UserId}", dto?.TeamId, dto?.UserId);
                    return new BaseResponse<List<TaskDto>>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Invalid team or user ID."
                    };
                }

                var isMember = await _teamUserRepository.FirstOrDefaultAsync(tu => tu.TeamId == dto.TeamId && tu.UserId == dto.UserId);
                if (isMember == null)
                {
                    _logger.LogWarning("User {UserId} is not a member of team {TeamId}", dto.UserId, dto.TeamId);
                    return new BaseResponse<List<TaskDto>>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Not a member of this team."
                    };
                }

                var tasksResponse = await _taskRepo.GetTeamTasks(dto);
                if (tasksResponse == null || !tasksResponse.Data.Any())
                {
                    _logger.LogWarning("No tasks found for team {TeamId}", dto.TeamId);
                    return new BaseResponse<List<TaskDto>>
                    {
                        ResponseCode = ResponseCodes.NOT_FOUND,
                        Message = "No tasks found for this team."
                    };
                }

                _logger.LogInformation("Successfully retrieved {TaskCount} tasks for team: {TeamId}", tasksResponse.Data.Count, dto.TeamId);
                return new BaseResponse<List<TaskDto>>
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Message = "Tasks retrieved successfully.",
                    Data = tasksResponse.Data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve tasks for team: {TeamId}, user: {UserId}", dto?.TeamId, dto?.UserId);
                return new BaseResponse<List<TaskDto>>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while retrieving tasks."
                };
            }
        }

        public async Task<BaseResponse<TaskDto>> CreateTaskAsync(string creatorId, CreateTaskDto dto)
        {
            _logger.LogInformation("Creating task for team: {TeamId}, creator: {CreatorId}", dto?.teamId, creatorId);

            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.teamId) || string.IsNullOrEmpty(creatorId) || dto.createTaskDto == null)
                {
                    _logger.LogWarning("CreateTaskAsync attempt with invalid DTO, TeamId: {TeamId}, CreatorId: {CreatorId}", dto?.teamId, creatorId);
                    return new BaseResponse<TaskDto>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Invalid task or team data."
                    };
                }

                var isMember = await _teamUserRepository.FirstOrDefaultAsync(tu => tu.TeamId == dto.teamId && tu.UserId == creatorId);
                if (isMember == null)
                {
                    _logger.LogWarning("User {CreatorId} is not a member of team {TeamId}", creatorId, dto.teamId);
                    return new BaseResponse<TaskDto>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "User is not a member of this team."
                    };
                }

                var task = new Task
                {
                    Title = dto.createTaskDto.Title,
                    Description = dto.createTaskDto.Description,
                    DueDate = dto.createTaskDto.DueDate,
                    AssignedToUserId = dto.createTaskDto.AssignedToUserId,
                    CreatedByUserId = creatorId,
                    TeamId = dto.teamId
                };

                await _taskRepository.AddAsync(task);
                await _unitOfWork.CommitChangesAsync();

                var taskDto = new TaskDto();
                taskDto.MapFrom(task);

                _logger.LogInformation("Task created successfully for team: {TeamId}, taskId: {TaskId}", dto.teamId, task.Id);
                return new BaseResponse<TaskDto>
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Message = "Task created successfully.",
                    Data = taskDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create task for team: {TeamId}, creator: {CreatorId}", dto?.teamId, creatorId);
                return new BaseResponse<TaskDto>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while creating the task."
                };
            }
        }

        public async Task<BaseResponse<bool>> UpdateTaskAsync(string taskId, TaskUpdateDto dto, string userId)
        {
            _logger.LogInformation("Updating task: {TaskId} by user: {UserId}", taskId, userId);

            try
            {
                if (string.IsNullOrEmpty(taskId) || string.IsNullOrEmpty(userId) || dto == null)
                {
                    _logger.LogWarning("UpdateTaskAsync attempt with invalid taskId: {TaskId}, userId: {UserId}, or DTO", taskId, userId);
                    return new BaseResponse<bool>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Invalid task or user data."
                    };
                }

                var task = await _taskRepository.GetByIdAsync(taskId);
                if (task == null || task.CreatedByUserId != userId)
                {
                    _logger.LogWarning("Task {TaskId} not found or user {UserId} lacks permission to update", taskId, userId);
                    return new BaseResponse<bool>
                    {
                        ResponseCode = ResponseCodes.NOT_FOUND,
                        Message = "Task not found or you do not have permission to update this task."
                    };
                }

                task.Title = dto.Title;
                task.Description = dto.Description;
                task.DueDate = dto.DueDate;
                task.AssignedToUserId = dto.AssignedToUserId;

                await _taskRepository.UpdateAsync(task);
                await _unitOfWork.CommitChangesAsync();

                _logger.LogInformation("Task {TaskId} updated successfully by user: {UserId}", taskId, userId);
                return new BaseResponse<bool>
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Message = "Task updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update task: {TaskId} by user: {UserId}", taskId, userId);
                return new BaseResponse<bool>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while updating the task."
                };
            }
        }

        public async Task<BaseResponse<bool>> DeleteTaskAsync(string taskId, string userId)
        {
            _logger.LogInformation("Deleting task: {TaskId} by user: {UserId}", taskId, userId);

            try
            {
                if (string.IsNullOrEmpty(taskId) || string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("DeleteTaskAsync attempt with invalid taskId: {TaskId} or userId: {UserId}", taskId, userId);
                    return new BaseResponse<bool>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Invalid task or user ID."
                    };
                }

                var task = await _taskRepository.GetByIdAsync(taskId);
                if (task == null || task.CreatedByUserId != userId)
                {
                    _logger.LogWarning("Task {TaskId} not found or user {UserId} lacks permission to delete", taskId, userId);
                    return new BaseResponse<bool>
                    {
                        ResponseCode = ResponseCodes.NOT_FOUND,
                        Message = "Task not found or you do not have permission to delete this task."
                    };
                }

                await _taskRepository.DeleteAsync(taskId);
                await _unitOfWork.CommitChangesAsync();

                _logger.LogInformation("Task {TaskId} deleted successfully by user: {UserId}", taskId, userId);
                return new BaseResponse<bool>
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Message = "Task deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete task: {TaskId} by user: {UserId}", taskId, userId);
                return new BaseResponse<bool>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while deleting the task."
                };
            }
        }

        public async Task<BaseResponse<bool>> UpdateTaskStatusAsync(string taskId, TaskStatusDto dto, string userId)
        {
            _logger.LogInformation("Updating status for task: {TaskId} by user: {UserId}", taskId, userId);

            try
            {
                if (string.IsNullOrEmpty(taskId) || string.IsNullOrEmpty(userId) || dto == null)
                {
                    _logger.LogWarning("UpdateTaskStatusAsync attempt with invalid taskId: {TaskId}, userId: {UserId}, or DTO", taskId, userId);
                    return new BaseResponse<bool>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Invalid task or user data."
                    };
                }

                var task = await _taskRepository.GetByIdAsync(taskId);
                if (task == null || task.AssignedToUserId != userId)
                {
                    _logger.LogWarning("Task {TaskId} not found or user {UserId} lacks permission to update status", taskId, userId);
                    return new BaseResponse<bool>
                    {
                        ResponseCode = ResponseCodes.NOT_FOUND,
                        Message = "Task not found or you do not have permission to update this task."
                    };
                }

                if (!Enum.TryParse<TaskStatus>(dto.Status, true, out var newStatus))
                {
                    _logger.LogWarning("Invalid task status: {Status} for task: {TaskId}", dto.Status, taskId);
                    return new BaseResponse<bool>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Invalid task status."
                    };
                }

                task.Status = (Enums.TaskStatus)newStatus;
                await _taskRepository.UpdateAsync(task);
                await _unitOfWork.CommitChangesAsync();

                _logger.LogInformation("Task {TaskId} status updated to {Status} by user: {UserId}", taskId, newStatus, userId);
                return new BaseResponse<bool>
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Message = "Task status updated successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update status for task: {TaskId} by user: {UserId}", taskId, userId);
                return new BaseResponse<bool>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while updating the task status."
                };
            }
        }
    }
}