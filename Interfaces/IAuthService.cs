using TeamTaskManagement.API.Response;

namespace TeamTaskManagement.API.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse<string>> RegisterAsync(RegisterDto dto);
        Task<BaseResponse<string>> LoginAsync(LoginDto dto);
        Task<UserDto> GetCurrentUserAsync(string userId);
    }
}
