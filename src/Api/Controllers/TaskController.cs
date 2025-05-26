using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using TeamTaskManagement.API.Interfaces;
using TeamTaskManagement.API.Response;

namespace TeamTaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [SwaggerOperation(Summary = $"get all teams task by teamId")]
        [ProducesResponseType(typeof(BaseResponse<TaskDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpGet("team/{teamId}")]
        public async Task<IActionResult> GetTasksForTeam(string teamId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var tasks = await _taskService.GetTasksAsync(teamId, userId);
            if (tasks.ResponseCode == ResponseCodes.SUCCESS) { return Ok(tasks); }
            return Forbid(userId);
        }

        [SwaggerOperation(Summary = "Create task in a team")]
        [ProducesResponseType(typeof(BaseResponse<TaskDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpPost("team/{teamId}")]
        public async Task<IActionResult> CreateTask(string teamId, [FromBody] TaskCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var response = await _taskService.CreateTaskAsync(teamId, userId, dto);
            if (response.ResponseCode == ResponseCodes.SUCCESS)
            {
                return CreatedAtAction(nameof(GetTaskById), new { taskId = response.Data.Id }, response);
            }

            return StatusCode(500, response);
        }

        // GET: api/tasks/{taskId}
        [SwaggerOperation(Summary = "get single task in a team by taskId")]
        [ProducesResponseType(typeof(BaseResponse<TaskDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskById(string taskId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var response = await _taskService.GetTaskByIdAsync(taskId, userId);
            if (response.ResponseCode == ResponseCodes.SUCCESS) { return Ok(response); }
            return StatusCode(500, response);
        }

        // PUT: api/tasks/{taskId}
        [SwaggerOperation(Summary = "Update task")]
        [ProducesResponseType(typeof(BaseResponse<TaskDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(string taskId, [FromBody] TaskUpdateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _taskService.UpdateTaskAsync(taskId, dto, userId);
            if (success.ResponseCode == ResponseCodes.SUCCESS) { return Ok(success); }
            return StatusCode(500, success);
        }

        // DELETE: api/tasks/{taskId}
        [SwaggerOperation(Summary = "Delete task by id")]
        [ProducesResponseType(typeof(BaseResponse<TaskDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(string taskId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _taskService.DeleteTaskAsync(taskId, userId);
            if (success.ResponseCode == ResponseCodes.SUCCESS) { return Ok(success); }
            return Forbid("Not authorized or task not found.");
        }

        // PATCH: api/tasks/{taskId}/status
        [SwaggerOperation(Summary = "Change Task status")]
        [ProducesResponseType(typeof(BaseResponse<TaskDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpPatch("{taskId}/status")]
        public async Task<IActionResult> UpdateTaskStatus(string taskId, [FromBody] TaskStatusDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _taskService.UpdateTaskStatusAsync(taskId, dto, userId);
            if (success.ResponseCode == ResponseCodes.SUCCESS) { return Ok(success); }
            return Forbid("Not authorized or task not found.");
        }
    }
}
