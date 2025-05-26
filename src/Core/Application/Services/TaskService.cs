using Microsoft.EntityFrameworkCore;
using TeamTaskManagement.API.Interfaces;
using TeamTaskManagementAPI.Data;

namespace TeamTaskManagement.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskDto>> GetTasksAsync(Guid teamId, string userId)
        {
            var guid = Guid.Parse(userId);
            var isMember = await _context.TeamUsers.AnyAsync(tu => tu.TeamId == teamId && tu.UserId == guid);
            if (!isMember) throw new UnauthorizedAccessException("Not a member of this team.");

            return await _context.Tasks
                .Where(t => t.TeamId == teamId)
                .Include(t => t.AssignedToUser)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    Status = t.Status.ToString(),
                    CreatedAt = t.CreatedAt,
                    AssignedToUserId = t.AssignedToUserId ?? Guid.Empty,
                    AssignedToUsername = t.AssignedToUser != null ? t.AssignedToUser.Username : "",
                    CreatedByUserId = t.CreatedByUserId,
                    TeamId = t.TeamId
                }).ToListAsync();
        }

        public async Task<TaskDto> CreateTaskAsync(Guid teamId, string creatorId, TaskCreateDto dto)
        {
            var guid = Guid.Parse(creatorId);
            var isMember = await _context.TeamUsers.AnyAsync(tu => tu.TeamId == teamId && tu.UserId == guid);
            if (!isMember) throw new UnauthorizedAccessException("Not a member of this team.");

            var task = new Models.Task
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                AssignedToUserId = dto.AssignedToUserId,
                CreatedByUserId = guid,
                TeamId = teamId
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Status = task.Status.ToString(),
                CreatedAt = task.CreatedAt,
                AssignedToUserId = task.AssignedToUserId ?? Guid.Empty,
                AssignedToUsername = "",
                CreatedByUserId = task.CreatedByUserId,
                TeamId = task.TeamId
            };
        }

        public async Task<bool> UpdateTaskAsync(Guid taskId, TaskUpdateDto dto, string userId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null || task.CreatedByUserId != Guid.Parse(userId))
                return false;

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.DueDate = dto.DueDate;
            task.AssignedToUserId = dto.AssignedToUserId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTaskAsync(Guid taskId, string userId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null || task.CreatedByUserId != Guid.Parse(userId))
                return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTaskStatusAsync(Guid taskId, TaskStatusDto dto, string userId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null || task.AssignedToUserId != Guid.Parse(userId))
                return false;

            if (!Enum.TryParse<TaskStatus>(dto.Status, true, out var newStatus))
                return false;

            task.Status = (Models.TaskStatus)newStatus;
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 