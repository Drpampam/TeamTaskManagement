using Application.Interfaces;
using Application.Interfaces.Application;
using Application.Responses;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAsyncRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;
        private readonly IJwtService JwtService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IAsyncRepository<User> userRepository,
            ILogger<AuthService> logger, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.JwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        }

        public async Task<BaseResponse<string>> RegisterAsync(RegisterDto dto)
        {
            _logger.LogInformation("Attempting to register user with email: {Email}", dto?.Email);

            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("Registration attempt with null DTO");
                    return new BaseResponse<string>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Invalid registration data."
                    };
                }

                var isEmailExists = await _userRepository.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (isEmailExists != null)
                {
                    _logger.LogWarning("Registration failed: Email {Email} already registered", dto.Email);
                    return new BaseResponse<string>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Email already registered."
                    };
                }

                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
                };

                await _userRepository.AddAsync(user);
                await _unitOfWork.CommitChangesAsync();

                _logger.LogInformation("User registered successfully: {Email}", dto.Email);
                return new BaseResponse<string>
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Data = JwtService.GenerateToken(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for email: {Email}", dto?.Email);
                return new BaseResponse<string>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while registering the user."
                };
            }
        }

        public async Task<BaseResponse<string>> LoginAsync(LoginDto dto)
        {
            _logger.LogInformation("Attempting login for email: {Email}", dto?.Email);

            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("Login attempt with null DTO");
                    return new BaseResponse<string>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Invalid login data."
                    };
                }

                var isEmailExists = await _userRepository.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (isEmailExists == null)
                {
                    _logger.LogWarning("Login failed: User with email {Email} not found", dto.Email);
                    return new BaseResponse<string>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "login failed: Not a user"
                    };
                }

                if (!BCrypt.Net.BCrypt.Verify(dto.Password, isEmailExists.PasswordHash))
                {
                    _logger.LogWarning("Login failed: Invalid password for email {Email}", dto.Email);
                    return new BaseResponse<string>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Invalid credentials."
                    };
                }

                _logger.LogInformation("User logged in successfully: {Email}", dto.Email);
                return new BaseResponse<string>
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Data = JwtService.GenerateToken(isEmailExists)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for email: {Email}", dto?.Email);
                return new BaseResponse<string>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while logging in."
                };
            }
        }

        public async Task<BaseResponse<UserDto>> GetCurrentUserAsync(string userId)
        {
            _logger.LogInformation("Retrieving user information for userId: {UserId}", userId);

            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("GetCurrentUser attempt with invalid userId");
                    return new BaseResponse<UserDto>
                    {
                        ResponseCode = ResponseCodes.VALIDATION_ERROR,
                        Message = "Invalid user ID."
                    };
                }

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found for userId: {UserId}", userId);
                    return new BaseResponse<UserDto>
                    {
                        ResponseCode = ResponseCodes.NOT_FOUND,
                        Message = "User not found."
                    };
                }

                var userDto = new UserDto();
                userDto.MapFromUser(user);

                _logger.LogInformation("Successfully retrieved user information for userId: {UserId}", userId);
                return new BaseResponse<UserDto>
                {
                    ResponseCode = ResponseCodes.SUCCESS,
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve user information for userId: {UserId}", userId);
                return new BaseResponse<UserDto>
                {
                    ResponseCode = ResponseCodes.SERVER_ERROR,
                    Message = "An error occurred while retrieving the user."
                };
            }
        }
    }
}