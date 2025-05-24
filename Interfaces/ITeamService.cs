using TeamTaskManagement.API.Response;

namespace TeamTaskManagement.API.Interfaces
{
    public interface ITeamService
    {
        Task<BaseResponse<TeamDto>> CreateTeamAsync(string creatorId, TeamCreateDto dto);
        //Task<BaseResponse<bool>> AddUserToTeamAsync(Guid teamId, string userEmail);
        Task<BaseResponse<string>> AddUserToTeamAsync(string teamId, string userEmail);
        Task<BaseResponse<List<TeamMemberDto>>> GetTeamMembersAsync(string teamId);
    }
}
