using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace TeamTaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("team")]
        [SwaggerOperation(
            Summary = "Gets all tasks for a team",
            Description = "Retrieves all tasks for the specified team ID, ensuring the user is a team member."
        )]
        [ProducesResponseType(typeof(BaseResponse<List<TaskDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<List<TaskDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<List<TaskDto>>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<List<TaskDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse<List<TaskDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTasksForTeam([FromBody] GetTaskDto dto)
        {
            var response = await _taskService.GetTasksAsync(dto);
            if (response.ResponseCode == ResponseCodes.SUCCESS) { return Ok(response); }
            return StatusCode(500, response);       
        }

        [HttpPost("create")]
        [SwaggerOperation(
            Summary = "Creates a new task in a team",
            Description = "Creates a task for the specified team, ensuring the creator is a team member."
        )]
        [ProducesResponseType(typeof(BaseResponse<TaskDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<TaskDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<TaskDto>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<TaskDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
        {
            var response = await _taskService.CreateTaskAsync(dto);
            if (response.ResponseCode == ResponseCodes.SUCCESS) { return Ok(response); }
            if (response.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(response); }
            return StatusCode(500, response);
        }

        [HttpPut("{taskId}")]
        [SwaggerOperation(
            Summary = "Updates a task",
            Description = "Updates the specified task, ensuring the user is the task creator."
        )]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateTask(string taskId, [FromBody] TaskUpdateDto dto)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var response = await _taskService.UpdateTaskAsync(taskId, dto, userId);
            if (response.ResponseCode == ResponseCodes.SUCCESS) { return Ok(response); }
            if (response.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(response); }
            return StatusCode(500, response);
        }

        [HttpDelete("{taskId}")]
        [SwaggerOperation(
            Summary = "Deletes a task",
            Description = "Deletes the specified task, ensuring the user is the task creator."
        )]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTask(string taskId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var response = await _taskService.DeleteTaskAsync(taskId, userId);
            if (response.ResponseCode == ResponseCodes.SUCCESS) { return Ok(response); }
            if (response.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(response); }
            return StatusCode(500, response);
        }

        [HttpPatch("{taskId}/status")]
        [SwaggerOperation(
            Summary = "Updates task status",
            Description = "Changes the status of the specified task, ensuring the user is the assigned user."
        )]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateTaskStatus(string taskId, [FromBody] TaskStatusDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var response = await _taskService.UpdateTaskStatusAsync(taskId, dto, userId);
            if (response.ResponseCode == ResponseCodes.SUCCESS) { return Ok(response); }
            if (response.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(response); }
            return StatusCode(500, response);
        }
    }
}