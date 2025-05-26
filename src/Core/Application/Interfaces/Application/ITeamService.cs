using Application.Responses;

namespace Application.Interfaces
{
    public interface ITeamService
    {
        Task<BaseResponse<TeamDto>> CreateTeamAsync(string creatorId, TeamCreateDto dto);
        Task<BaseResponse<bool>> AddUserToTeamAsync(string teamId, string userEmail);
        Task<BaseResponse<List<TeamMemberDto>>> GetTeamMembersAsync(string teamId);
        Task<BaseResponse<List<CurrentTeam>>> GetUserTeam(string userId);
    }
}
