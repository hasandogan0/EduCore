using AutoMapper;
using EduCore.Business.DTOs;
using EduCore.Business.Services.Abstract;
using EduCore.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EduCore.Business.Services.Concrete;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper; // AutoMapper eklendi

    public AuthService(UserManager<ApplicationUser> userManager,
                       RoleManager<IdentityRole> roleManager,
                       IConfiguration configuration,
                       IMapper mapper) // Inject edildi
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        if (registerDto.Role != "Student" && registerDto.Role != "Instructor")
        {
            throw new Exception("Geçersiz rol seçimi.");
        }

        // --- AutoMapper Kullanımı ---
        var user = _mapper.Map<ApplicationUser>(registerDto);

        // Manuel atanması gereken özel mantıklar
        user.IsApproved = registerDto.Role == "Student";

        // Dosya Yükleme İşlemi (Aynı kaldı)
        if (registerDto.ProfileImage != null && registerDto.ProfileImage.Length > 0)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(registerDto.ProfileImage.FileName);
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");

            if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

            var filePath = Path.Combine(uploadDir, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await registerDto.ProfileImage.CopyToAsync(stream);
            }
            user.HeadshotUrl = $"/uploads/profiles/{fileName}";
        }

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Kayıt başarısız: {errors}");
        }

        await _userManager.AddToRoleAsync(user, registerDto.Role);

        if (registerDto.Role == "Instructor")
        {
            // --- AutoMapper Kullanımı ---
            var response = _mapper.Map<AuthResponseDto>(user);
            response.Token = ""; // Henüz onaylanmadığı için boş token
            response.Role = "Instructor";
            return response;
        }

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            throw new Exception("Geçersiz kullanıcı adı veya şifre.");
        }

        if (!user.IsActive) throw new Exception("Hesabınız devre dışı bırakılmıştır.");

        if (!user.IsApproved) throw new Exception("Hesabınız henüz onaylanmamış. Lütfen yönetici onayını bekleyin.");

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> GetCurrentUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new Exception("Kullanıcı bulunamadı.");
        return await GenerateAuthResponseAsync(user);
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Student";

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Role, role),
            new Claim("IsApproved", user.IsApproved.ToString())
        };

        var jwtKey = _configuration["Jwt:Key"] ?? "BuCokGizliVeUzunBirAnahtarOlmalidir123456!";
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        // --- AutoMapper Kullanımı ---
        var response = _mapper.Map<AuthResponseDto>(user);
        response.Token = new JwtSecurityTokenHandler().WriteToken(token);
        response.Role = role;

        return response;
    }
}