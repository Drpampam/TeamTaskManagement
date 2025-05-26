namespace Application.Interfaces
{
    public interface ITeamService
    {
        Task<TeamDto> CreateTeamAsync(string creatorId, TeamCreateDto dto);
        Task<bool> AddUserToTeamAsync(Guid teamId, string userEmail);
        Task<List<TeamMemberDto>> GetTeamMembersAsync(Guid teamId);
    }
}
