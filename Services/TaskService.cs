using Microsoft.EntityFrameworkCore;
using TeamTaskManagement.API.Interfaces;
using TeamTaskManagement.API.Response;
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

        public async Task<BaseResponse<List<TaskDto>>> GetTasksAsync(Guid teamId, string userId)
        {
            try
            {
                var guid = Guid.Parse(userId);
                var isMember = await _context.TeamUsers.AnyAsync(tu => tu.TeamId == teamId && tu.UserId == guid);
                if (isMember)
                {
                    var team = await _context.Teams
                        .Include(t => t.TeamUsers)
                        .ThenInclude(tu => tu.User)
                        .FirstOrDefaultAsync(t => t.Id == teamId);
                    if (team == null)
                    {
                        return new BaseResponse<List<TaskDto>>
                        {
                            Message = "Team not found.",
                            ResponseCode = ResponseCodes.FAILURE
                        };
                    }
                    var user = team.TeamUsers.FirstOrDefault(tu => tu.UserId == guid);
                    if (user == null)
                    {
                        return new BaseResponse<List<TaskDto>>
                        {
                            Message = "User not found in team.",
                            ResponseCode = ResponseCodes.FAILURE
                        };
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
                            AssignedToUserId = t.AssignedToUserId ?? Guid.Empty,
                            AssignedToUsername = "",
                            CreatedByUserId = t.CreatedByUserId,
                            TeamId = t.TeamId
                        })
                        .ToListAsync();
                    return new BaseResponse<List<TaskDto>>
                    {
                        Data = tasks,
                        Message = "Successful",
                        ResponseCode = ResponseCodes.SUCCESS
                    };
                }                  
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<TaskDto>>
                {
                    Message = "An error occurred {ex}... Please contact support" + ex.Message,
                    ResponseCode = ResponseCodes.SERVER_ERROR
                };
            }    
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