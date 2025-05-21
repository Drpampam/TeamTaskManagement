using TeamTaskManagement.API.Response;

namespace TeamTaskManagement.API.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse<string>> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
        Task<UserDto> GetCurrentUserAsync(string userId);
    }
}
