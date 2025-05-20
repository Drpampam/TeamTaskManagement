using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamTaskManagement.API.Interfaces;

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
        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] TeamCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var team = await _teamService.CreateTeamAsync(userId, dto);
            return CreatedAtAction(nameof(GetTeamMembers), new { teamId = team.Id }, team);
        }

        // POST: api/teams/{teamId}/users
        [HttpPost("{teamId}/users")]
        public async Task<IActionResult> AddUserToTeam(Guid teamId, [FromBody] string userEmail)
        {
            var success = await _teamService.AddUserToTeamAsync(teamId, userEmail);
            if (!success)
                return BadRequest("User not found or already in team.");

            return NoContent();
        }

        // GET: api/teams/{teamId}/members
        [HttpGet("{teamId}/members")]
        public async Task<IActionResult> GetTeamMembers(Guid teamId)
        {
            var members = await _teamService.GetTeamMembersAsync(teamId);
            return Ok(members);
        }
    }
}
