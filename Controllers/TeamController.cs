using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using TeamTaskManagement.API.Interfaces;
using TeamTaskManagement.API.Response;

namespace TeamTaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        // POST: api/teams
        [SwaggerOperation(Summary = $"Create new team")]
        [ProducesResponseType(typeof(BaseResponse<TeamDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] TeamCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var team = await _teamService.CreateTeamAsync(userId, dto);
            return CreatedAtAction(nameof(GetTeamMembers), new { teamId = team.Id }, team);
        }

        // POST: api/teams/{teamId}/users
        [SwaggerOperation(Summary = $"add a user to team")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpPost("{teamId}/users")]
        public async Task<IActionResult> AddUserToTeam(Guid teamId, [FromBody] string userEmail)
        {
            var success = await _teamService.AddUserToTeamAsync(teamId, userEmail);
            if (success.ResponseCode == ResponseCodes.SUCCESS) { return Ok(success.Data); }
            return StatusCode(500, success);
        }

        // GET: api/teams/{teamId}/members
        [SwaggerOperation(Summary = $"get team members")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpGet("{teamId}/members")]
        public async Task<IActionResult> GetTeamMembers(Guid teamId)
        {
            var members = await _teamService.GetTeamMembersAsync(teamId);
            if (members.ResponseCode == ResponseCodes.SUCCESS) { return Ok(members); }
            return StatusCode(500, members);
        }

    }
}
