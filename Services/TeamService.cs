using Microsoft.EntityFrameworkCore;
using TeamTaskManagement.API.Interfaces;
using TeamTaskManagement.API.Models;
using TeamTaskManagement.API.Response;
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

        public async Task<BaseResponse<TeamDto>> CreateTeamAsync(string creatorId, TeamCreateDto dto)
        {
            try
            {
                var team = new Team { Name = dto.Name };
                _context.Teams.Add(team);

                var teamUser = new TeamUser
                {
                    Team = team,
                    UserId = creatorId,
                    Role = TeamRole.Admin
                };

                _context.TeamUsers.Add(teamUser);
                await _context.SaveChangesAsync();

                var result = new TeamDto
                {
                    Id = team.Id,
                    Name = team.Name,
                    CreatedAt = DateTime.UtcNow
                };

                return new BaseResponse<TeamDto>(
                    message: "Team created successfully",
                    data: result,
                    responseCode: "00"
                );
            }
            catch (Exception ex)
            {
                // Optional: log exception
                return new BaseResponse<TeamDto>("An error occurred while creating the team", "99");
            }
        }

        public async Task<BaseResponse<string>> AddUserToTeamAsync(string teamId, string userEmail)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                if (user == null)
                {
                    return new BaseResponse<string>("User not found", "01");
                }

                var exists = await _context.TeamUsers
                    .AnyAsync(tu => tu.TeamId == teamId && tu.UserId == user.Id);

                if (exists)
                {
                    return new BaseResponse<string>("User is already a member of the team", "02");
                }

                _context.TeamUsers.Add(new TeamUser
                {
                    TeamId = teamId,
                    UserId = user.Id,
                    Role = TeamRole.Member
                });

                await _context.SaveChangesAsync();

                return new BaseResponse<string>("User added to team successfully", "00")
                {
                    Data = "Success"
                };
            }
            catch (Exception ex)
            {
                // Optional: log exception
                return new BaseResponse<string>("An error occurred while adding user to team", "99");
            }
        }

        public async Task<BaseResponse<List<TeamMemberDto>>> GetTeamMembersAsync(string teamId)
        {
            try
            {
                var members = await _context.TeamUsers
                    .Where(tu => tu.TeamId == teamId)
                    .Include(tu => tu.User)
                    .Select(tu => new TeamMemberDto
                    {
                        UserId = tu.UserId,
                        Username = tu.User.Username,
                        Email = tu.User.Email,
                        Role = tu.Role.ToString()
                    })
                    .ToListAsync();

                return new BaseResponse<List<TeamMemberDto>>(
                    message: "Team members retrieved successfully",
                    data: members,
                    responseCode: "00"
                );
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<TeamMemberDto>>("Failed to retrieve team members", "99");
            }
        }
    }
}
