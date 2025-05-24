using Microsoft.EntityFrameworkCore;
using TeamTaskManagement.API.Interfaces;
using TeamTaskManagement.API.Models;
using TeamTaskManagement.API.Response;
using TeamTaskManagementAPI.Data;
using TaskStatus = TeamTaskManagement.API.Models.TaskStatus;

namespace TeamTaskManagement.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BaseResponse<List<TaskDto>>> GetTasksAsync(string teamId, string userId)
        {
            try
            {
                var userGuid = Guid.Parse(userId);
                var isMember = await _context.TeamUsers.AnyAsync(tu => tu.TeamId == teamId && tu.UserId == userId);
                if (!isMember)
                {
                    return new BaseResponse<List<TaskDto>>("User is not a member of the team.", "01");
                }

                var teamExists = await _context.Teams.AnyAsync(t => t.Id == teamId);
                if (!teamExists)
                {
                    return new BaseResponse<List<TaskDto>>("Team not found.", "01");
                }

                var tasks = await _context.Tasks
                    .Where(t => t.TeamId == teamId)
                    .Select(t => new TaskDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        DueDate = t.DueDate,
                        Status = t.Status.ToString(),
                        CreatedAt = t.CreatedAt,
                        AssignedToUserId = t.AssignedToUserId ?? string.Empty,
                        AssignedToUsername = "", // Optional enhancement: join to fetch username
                        CreatedByUserId = t.CreatedByUserId,
                        TeamId = t.TeamId
                    })
                    .ToListAsync();

                return new BaseResponse<List<TaskDto>>("Tasks retrieved successfully.", tasks, "00");
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<TaskDto>>("An error occurred while retrieving tasks: " + ex.Message, "99");
            }
        }

        public async Task<BaseResponse<TaskDto>> CreateTaskAsync(string teamId, string creatorId, TaskCreateDto dto)
        {
            try
            {
                var creatorGuid = Guid.Parse(creatorId);
                var isMember = await _context.TeamUsers.AnyAsync(tu => tu.TeamId == teamId && tu.UserId == creatorId);

                if (!isMember)
                {
                    return new BaseResponse<TaskDto>("User is not authorized to create tasks in this team.", "01");
                }

                var task = new Models.Task
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    DueDate = dto.DueDate,
                    AssignedToUserId = dto.AssignedToUserId,
                    CreatedByUserId = creatorId,
                    TeamId = teamId
                };

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                var response = new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Status = task.Status.ToString(),
                    CreatedAt = task.CreatedAt,
                    AssignedToUserId = task.AssignedToUserId ?? string.Empty,
                    AssignedToUsername = "",
                    CreatedByUserId = task.CreatedByUserId,
                    TeamId = task.TeamId
                };

                return new BaseResponse<TaskDto>("Task created successfully.", response, "00");
            }
            catch (Exception ex)
            {
                return new BaseResponse<TaskDto>("An error occurred while creating task: " + ex.Message, "99");
            }
        }

        public async Task<BaseResponse<bool>> UpdateTaskAsync(string taskId, TaskUpdateDto dto, string userId)
        {
            try
            {
                var userGuid = Guid.Parse(userId);
                var task = await _context.Tasks.FindAsync(taskId);

                if (task == null)
                    return new BaseResponse<bool>("Task not found.", false, "01");

                if (task.CreatedByUserId != userId)
                    return new BaseResponse<bool>("Unauthorized access.", false, "03");

                task.Title = dto.Title;
                task.Description = dto.Description;
                task.DueDate = dto.DueDate;
                task.AssignedToUserId = dto.AssignedToUserId;

                await _context.SaveChangesAsync();
                return new BaseResponse<bool>("Task updated successfully.", true, "00");
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>("An error occurred while updating task: " + ex.Message, false, "99");
            }
        }

        public async Task<BaseResponse<bool>> DeleteTaskAsync(string taskId, string userId)
        {
            try
            {
                var userGuid = Guid.Parse(userId);
                var task = await _context.Tasks.FindAsync(taskId);

                if (task == null)
                    return new BaseResponse<bool>("Task not found.", false, "01");

                if (task.CreatedByUserId != userId)
                    return new BaseResponse<bool>("Unauthorized access.", false, "03");

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                return new BaseResponse<bool>("Task deleted successfully.", true, "00");
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>("An error occurred while deleting task: " + ex.Message, false, "99");
            }
        }

        public async Task<BaseResponse<bool>> UpdateTaskStatusAsync(string taskId, TaskStatusDto dto, string userId)
        {
            try
            {
                var userGuid = Guid.Parse(userId);
                var task = await _context.Tasks.FindAsync(taskId);

                if (task == null)
                    return new BaseResponse<bool>("Task not found.", false, "01");

                if (task.AssignedToUserId != userId)
                    return new BaseResponse<bool>("Only the assigned user can update the task status.", false, "03");

                if (!Enum.TryParse<TaskStatus>(dto.Status, true, out var newStatus))
                    return new BaseResponse<bool>("Invalid task status value.", false, "04");

                task.Status = (Models.TaskStatus)newStatus;
                await _context.SaveChangesAsync();

                return new BaseResponse<bool>("Task status updated successfully.", true, "00");
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>("An error occurred while updating task status: " + ex.Message, false, "99");
            }
        }
    }
}
