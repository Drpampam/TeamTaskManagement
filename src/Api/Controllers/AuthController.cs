using Application.Interfaces;
using Application.Responses;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

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
            if (token.ResponseCode == ResponseCodes.SUCCESS) { return Ok(new { Token = token}); }
            if (token.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(token); }
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
            if (token.ResponseCode == ResponseCodes.SUCCESS) { return Ok(new { Token = token }); }
            if (token.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(token); }
            return StatusCode(500, token);
        }

        //[Authorize]
        //[SwaggerOperation(Summary = $"get all users")]
        //[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status500InternalServerError)]
        //[HttpPost("getAllUsers")]
        //public async Task<IActionResult> GetUsers([FromBody] GetAllUsers req)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized();

        //    var token = await _authService.GetAllActiveUsers(req, userId);
        //    if (token.ResponseCode == ResponseCodes.SUCCESS) { return Ok(new { Token = token.Data }); }
        //    return StatusCode(500, token);
        //}

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
            if (userDto.ResponseCode == ResponseCodes.SUCCESS) { return Ok(userDto); }
            if (userDto.ResponseCode != ResponseCodes.SERVER_ERROR) { return BadRequest(userDto); }
            return StatusCode(500, userDto.Data);
        }
    }
}
