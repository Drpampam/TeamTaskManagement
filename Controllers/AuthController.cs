using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using TeamTaskManagement.API.Interfaces;
using TeamTaskManagement.API.Response;

namespace TeamTaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [SwaggerOperation(Summary = $"Register new user")]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var token = await _authService.RegisterAsync(dto);
            if (token.ResponseCode == ResponseCodes.SUCCESS) { return Ok(new { Token = token.Data }); }
            return StatusCode(500, token);
        }

        [SwaggerOperation(Summary = $"user login")]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token.ResponseCode == ResponseCodes.SUCCESS) { return Ok(new { Token = token.Data }); }
            return StatusCode(500, token);
        }

        [Authorize]
        [SwaggerOperation(Summary = $"current user")]
        [ProducesResponseType(typeof(BaseResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var userDto = await _authService.GetCurrentUserAsync(userId);
            if (userDto.ResponseCode == ResponseCodes.SUCCESS) { return Ok(userDto.Data); }
            return StatusCode(500, userDto.Data);
        }
    }
}
