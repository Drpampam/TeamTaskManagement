using Microsoft.EntityFrameworkCore;
using TeamTaskManagement.API.Interfaces;
using TeamTaskManagement.API.Models;
using TeamTaskManagementAPI.Data;

namespace TeamTaskManagement.API.Services
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _context;

        public TeamService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TeamDto> CreateTeamAsync(string creatorId, TeamCreateDto dto)
        {
            var team = new Team { Name = dto.Name };
            _context.Teams.Add(team);

            var teamUser = new TeamUser
            {
                Team = team,
                UserId = Guid.Parse(creatorId),
                Role = TeamRole.Admin
            };

            _context.TeamUsers.Add(teamUser);
            await _context.SaveChangesAsync();

            return new TeamDto { Name = team.Name, Id = team.Id , CreatedAt = DateTime.Now};
        }

        public async Task<bool> AddUserToTeamAsync(Guid teamId, string userEmail)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return false;

            var exists = await _context.TeamUsers.AnyAsync(tu => tu.TeamId == teamId && tu.UserId == user.Id);
            if (exists) return false;

            _context.TeamUsers.Add(new TeamUser
            {
                TeamId = teamId,
                UserId = user.Id,
                Role = TeamRole.Member
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TeamMemberDto>> GetTeamMembersAsync(Guid teamId)
        {
            return await _context.TeamUsers
                .Where(tu => tu.TeamId == teamId)
                .Include(tu => tu.User)
                .Select(tu => new TeamMemberDto
                {
                    UserId = tu.UserId,
                    Username = tu.User.Username,
                    Email = tu.User.Email,
                    Role = tu.Role.ToString()
                }).ToListAsync();
        }
    }
} 