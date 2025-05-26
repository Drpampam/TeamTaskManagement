using Application.Interfaces;
using Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

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
            if(team.ResponseCode == ResponseCodes.SUCCESS) return CreatedAtAction(nameof(GetTeamMembers), new { teamId = team.Data.Id }, team);
            if (team.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(team); }
            return StatusCode(500, team);
        }

        [SwaggerOperation(Summary = $"add a user to team")]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpPost("AddMembers")]
        public async Task<IActionResult> AddUserToTeam([FromBody] AddTeamMembers req)
        {
            var success = await _teamService.AddUserToTeamAsync(req.TeamId, req.MemberEmail);
            if (success.ResponseCode == ResponseCodes.SUCCESS) { return Ok(success); }
            if (success.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(success); }
            return StatusCode(500, success);
        }

        [SwaggerOperation(Summary = $"get team members")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpGet("{teamId}/members")]
        public async Task<IActionResult> GetTeamMembers(string teamId)
        {
            var members = await _teamService.GetTeamMembersAsync(teamId);
            if (members.ResponseCode == ResponseCodes.SUCCESS) { return Ok(members); }
            if (members.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(members); }
            return StatusCode(500, members);
        }

        [SwaggerOperation(Summary = $"get team user belongs")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpGet("teams")]
        public async Task<IActionResult> GetTeam()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var members = await _teamService.GetUserTeam(userId);
            if (members.ResponseCode == ResponseCodes.SUCCESS) { return Ok(members); }
            if (members.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(members); }
            return StatusCode(500, members);
        }
    }
}
