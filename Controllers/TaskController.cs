using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagement.API.Interfaces;
using TeamTaskManagement.API.Models;

namespace TeamTaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // GET: api/tasks/team/{teamId}
        [HttpGet("team/{teamId}")]
        public async Task<IActionResult> GetTasksForTeam(Guid teamId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                var tasks = await _taskService.GetTasksAsync(teamId, userId);
                return Ok(tasks);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // POST: api/tasks/team/{teamId}
        [HttpPost("team/{teamId}")]
        public async Task<IActionResult> CreateTask(Guid teamId, [FromBody] TaskCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                var task = await _taskService.CreateTaskAsync(teamId, userId, dto);
                return CreatedAtAction(nameof(GetTaskById), new { taskId = task.Id }, task);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // GET: api/tasks/{taskId}
        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskById(Guid taskId)
        {
            // This method isn't in your service, but might be useful.
            // You can implement it in your TaskService or skip it here.
            return NotFound();
        }

        // PUT: api/tasks/{taskId}
        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] TaskUpdateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _taskService.UpdateTaskAsync(taskId, dto, userId);
            if (!success) return Forbid("Not authorized or task not found.");

            return NoContent();
        }

        // DELETE: api/tasks/{taskId}
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(Guid taskId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _taskService.DeleteTaskAsync(taskId, userId);
            if (!success) return Forbid("Not authorized or task not found.");

            return NoContent();
        }

        // PATCH: api/tasks/{taskId}/status
        [HttpPatch("{taskId}/status")]
        public async Task<IActionResult> UpdateTaskStatus(Guid taskId, [FromBody] TaskStatusDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _taskService.UpdateTaskStatusAsync(taskId, dto, userId);
            if (!success) return Forbid("Not authorized, invalid status, or task not found.");

            return NoContent();
        }
    }
}
