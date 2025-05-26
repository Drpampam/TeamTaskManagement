using Application.Responses;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<BaseResponse<string>> RegisterAsync(RegisterDto dto);
        Task<BaseResponse<string>> LoginAsync(LoginDto dto);
        Task<BaseResponse<UserDto>> GetCurrentUserAsync(string userId);
    }
}
