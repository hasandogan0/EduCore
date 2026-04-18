using EduCore.Business.DTOs;
using EduCore.Business.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduCore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
    {
        try
        {
            // Servis artık RegisterDto -> ApplicationUser dönüşümünü AutoMapper ile yapıyor
            var response = await _authService.RegisterAsync(registerDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var response = await _authService.LoginAsync(loginDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            // Login hatalarında Unauthorized dönmek daha güvenlidir
            return Unauthorized(new { Message = ex.Message });
        }
    }

    [HttpGet("me")]
    [Authorize] // Artık tam namespace kullanmaya gerek yok, yukarıya using ekledik
    public async Task<IActionResult> GetMe()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var response = await _authService.GetCurrentUserAsync(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}