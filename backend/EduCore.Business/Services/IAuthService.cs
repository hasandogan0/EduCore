using EduCore.Business.DTOs;

namespace EduCore.Business.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> GetCurrentUserAsync(string userId);
    }
}
