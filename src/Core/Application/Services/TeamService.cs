using Application.Interfaces;
using Application.Responses;
using Domain.Common;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Domain.Common.Enums;

namespace TeamTaskManagement.API.Services
{
    public class TeamService : ITeamService
    {
        private readonly IAsyncRepository<Team> _teamRepository;
        private readonly IAsyncRepository<User> _userRepository;
        private readonly IAsyncRepository<TeamUser> _teamUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TeamService> _logger;

        public TeamService(
            IAsyncRepository<Team> teamRepository,
            IAsyncRepository<User> userRepository,
            IAsyncRepository<TeamUser> teamUserRepository,
            IUnitOfWork unitOfWork,
            ILogger<TeamService> logger)
        {
            _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _teamUserRepository = teamUserRepository ?? throw new ArgumentNullException(nameof(teamUserRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BaseResponse<TeamDto>> CreateTeamAsync(string creatorId, TeamCreateDto dto)
        {
            _logger.LogInformation("Creating team for creator: {CreatorId}, team name: {TeamName}", creatorId, dto?.Name);

            try
            {
                if (string.IsNullOrEmpty(creatorId) || dto == null || string.IsNullOrEmpty(dto.Name))
                {
                    _logger.LogWarning("CreateTeamAsync attempt with invalid creatorId: {CreatorId} or DTO name: {TeamName}", creatorId, dto?.Name);
                    return new BaseResponse<TeamDto>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Invalid creator ID or team name."
                    };
                }

                var user = await _userRepository.SingleOrDefaultAsync(u => u.Id == creatorId);
                if (user == null)
                {
                    _logger.LogWarning("User not found for creatorId: {CreatorId}", creatorId);
                    return new BaseResponse<TeamDto>
                    {
                        ResponseCode = ResponseCodes.NOT_FOUND,
                        Message = "User not found."
                    };
                }

                var existingTeam = await _teamRepository.SingleOrDefaultAsync(t => t.Name == dto.Name);
                if (existingTeam != null)
                {
                    _logger.LogWarning("Team with name {TeamName} already exists", dto.Name);
                    return new BaseResponse<TeamDto>
                    {
                        ResponseCode = ResponseCodes.DUPLICATE_RESOURCE,
                        Message = "Team with this name already exists."
                    };
                }

                var team = new Team { Name = dto.Name };
                await _teamRepository.AddAsync(team);
                await _unitOfWork.CommitChangesAsync();

                var teamUser = new TeamUser
                {
                    TeamId = team.Id,
                    UserId = creatorId,
                    Role = TeamRole.Admin
                };
                await _teamUserRepository.AddAsync(teamUser);
                await _unitOfWork.CommitChangesAsync();

                var teamDto = new TeamDto
                {
                    Id = team.Id,
                    Name = team.Name,
                    CreatedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Team created successfully: {TeamId}, name: {TeamName}", team.Id, team.Name);
                return new BaseResponse<TeamDto>
                {
                    ResponseCode = ResponseCodes.CREATED,
                    Message = "Team created successfully.",
                    Data = teamDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create team for creator: {CreatorId}, name: {TeamName}", creatorId, dto?.Name);
                return new BaseResponse<TeamDto>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while creating the team."
                };
            }
        }

        public async Task<BaseResponse<bool>> AddUserToTeamAsync(string teamId, string userEmail)
        {
            _logger.LogInformation("Adding user with email: {UserEmail} to team: {TeamId}", userEmail, teamId);

            try
            {
                if (string.IsNullOrEmpty(teamId) || string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogWarning("AddUserToTeamAsync attempt with invalid teamId: {TeamId} or userEmail: {UserEmail}", teamId, userEmail);
                    return new BaseResponse<bool>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Invalid team ID or user email."
                    };
                }

                var team = await _teamRepository.GetByIdAsync(teamId);
                if (team == null)
                {
                    _logger.LogWarning("Team not found for teamId: {TeamId}", teamId);
                    return new BaseResponse<bool>
                    {
                        ResponseCode = ResponseCodes.NOT_FOUND,
                        Message = "Team not found."
                    };
                }

                var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == userEmail);
                if (user == null)
                {
                    _logger.LogWarning("User not found for email: {UserEmail}", userEmail);
                    return new BaseResponse<bool>
                    {
                        ResponseCode = ResponseCodes.NOT_FOUND,
                        Message = "User not found."
                    };
                }

                var exists = await _teamUserRepository.FirstOrDefaultAsync(tu => tu.TeamId == teamId && tu.UserId == user.Id);
                if (exists != null)
                {
                    _logger.LogWarning("User {UserId} is already a member of team {TeamId}", user.Id, teamId);
                    return new BaseResponse<bool>
                    {
                        ResponseCode = ResponseCodes.DUPLICATE_RESOURCE,
                        Message = "User is already a member of this team."
                    };
                }

                var teamUser = new TeamUser
                {
                    TeamId = teamId,
                    UserId = user.Id,
                    Role = TeamRole.Member
                };
                await _teamUserRepository.AddAsync(teamUser);
                await _unitOfWork.CommitChangesAsync();

                _logger.LogInformation("User {UserId} added to team {TeamId} successfully", user.Id, teamId);
                return new BaseResponse<bool>
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Message = "User added to team successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add user with email: {UserEmail} to team: {TeamId}", userEmail, teamId);
                return new BaseResponse<bool>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while adding the user to the team."
                };
            }
        }

        public async Task<BaseResponse<List<TeamMemberDto>>> GetTeamMembersAsync(string teamId)
        {
            _logger.LogInformation("Retrieving members for team: {TeamId}", teamId);

            try
            {
                if (string.IsNullOrEmpty(teamId))
                {
                    _logger.LogWarning("GetTeamMembersAsync attempt with invalid teamId: {TeamId}", teamId);
                    return new BaseResponse<List<TeamMemberDto>>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Invalid team ID."
                    };
                }

                var team = await _teamRepository.GetByIdAsync(teamId);
                if (team == null)
                {
                    _logger.LogWarning("Team not found for teamId: {TeamId}", teamId);
                    return new BaseResponse<List<TeamMemberDto>>
                    {
                        ResponseCode = ResponseCodes.NOT_FOUND,
                        Message = "Team not found."
                    };
                }

                var teamMembers = await _teamUserRepository
                    .WhereQueryable(tu => tu.TeamId == teamId)
                    .Result
                    .Include(tu => tu.User)
                    .Select(tu => new TeamMemberDto
                    {
                        UserId = tu.UserId,
                        Username = tu.User.Username,
                        Email = tu.User.Email,
                        Role = tu.Role.ToString()
                    })
                    .ToListAsync();

                if (!teamMembers.Any())
                {
                    _logger.LogWarning("No members found for team: {TeamId}", teamId);
                    return new BaseResponse<List<TeamMemberDto>>
                    {
                        ResponseCode = ResponseCodes.NOT_FOUND,
                        Message = "No members found for this team."
                    };
                }

                _logger.LogInformation("Successfully retrieved {MemberCount} members for team: {TeamId}", teamMembers.Count, teamId);
                return new BaseResponse<List<TeamMemberDto>>
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Message = "Team members retrieved successfully.",
                    Data = teamMembers
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve members for team: {TeamId}", teamId);
                return new BaseResponse<List<TeamMemberDto>>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while retrieving team members."
                };
            }
        }

        public async Task<BaseResponse<List<CurrentTeam>>> GetUserTeam(string userId)
        {
            _logger.LogInformation("Retrieving teams by: {userId}", userId);

            try
            { 
                var teamMembers = await _teamUserRepository
                    .WhereQueryable(tu => tu.UserId == userId)
                    .Result
                    .Include(tu => tu.User)
                    .Select(tu => new CurrentTeam
                    {
                        UserId = tu.UserId ?? tu.User.Id,
                        TeamId = tu.TeamId,
                        TeamName = tu.Team.Name,
                        Role = tu.Role.ToString()
                    })
                    .ToListAsync();

                if (!teamMembers.Any())
                {
                    _logger.LogWarning("Not in a team yet: Create new team");
                    return new BaseResponse<List<CurrentTeam>>
                    {
                        ResponseCode = ResponseCodes.NOT_FOUND,
                        Message = "Not in a team yet: Create new team"
                    };
                }

                _logger.LogInformation("Successfully retrieved {Team} for team: {TeamId}", teamMembers.Count, userId);
                return new BaseResponse<List<CurrentTeam>>
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Message = "Team retrieved successfully.",
                    Data = teamMembers
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Successfully retrieved {Team} for team: {TeamId}", "Failed to retrieve members for team: {TeamId}", userId);
                return new BaseResponse<List<CurrentTeam>>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while retrieving team members."
                };
            }
        }
    }
}