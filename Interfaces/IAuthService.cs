namespace TeamTaskManagement.API.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
        Task<UserDto> GetCurrentUserAsync(string userId);
    }
}
