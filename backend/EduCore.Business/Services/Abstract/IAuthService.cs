using EduCore.Business.DTOs;

namespace EduCore.Business.Services.Abstract
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> GetCurrentUserAsync(string userId);
    }
}
