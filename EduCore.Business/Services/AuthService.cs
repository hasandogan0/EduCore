using EduCore.Business.DTOs;
using EduCore.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EduCore.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto.Role != "Student" && registerDto.Role != "Instructor")
            {
                throw new Exception("Invalid role selected");
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                IsApproved = registerDto.Role == "Student" // Instructors need approval
            };

            if (registerDto.ProfileImage != null && registerDto.ProfileImage.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(registerDto.ProfileImage.FileName);
                var uploadDir = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
                
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                var filePath = System.IO.Path.Combine(uploadDir, fileName);
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
                throw new Exception($"Registration failed: {errors}");
            }

            await _userManager.AddToRoleAsync(user, registerDto.Role);

            // If Instructor, they might not get a token immediately if we block them? 
            // For now, let's give them a token but they can't do anything because of Policy checks we will add.
            // OR we can throw exception "Registration successful, please wait for approval".
            
            // Requirements said: "Instructors will require SuperAdmin approval... to access course creation features."
            // So they can probably login, but "Create Course" will fail.
            
            if (registerDto.Role == "Instructor")
            {
                // Instructors do not get a token immediately.
                // We return empty token/response or indicate success but pending approval.
                return new AuthResponseDto
                {
                    Token = "", 
                    Username = user.UserName ?? "",
                    Email = user.Email ?? "",
                    Role = "Instructor" // Frontend should check if Token is empty to show "Pending Approval" message
                };
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

            if (!user.IsActive)
            {
                throw new Exception("Hesabınız devre dışı bırakılmıştır.");
            }

            if (!user.IsApproved)
            {
                throw new Exception("Hesabınız henüz onaylanmamış. Lütfen yönetici onayını bekleyin.");
            }

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponseDto> GetCurrentUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new Exception("User not found");
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

            var jwtKey = _configuration["Jwt:Key"] ?? "PleaseProvideASecretKeyHereThatIsLongEnough123456789"; 
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "EduCore",
                audience: _configuration["Jwt:Audience"] ?? "EduCoreUser",
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Username = user.UserName ?? "",
                Email = user.Email ?? "",
                Role = role,
                FullName = user.FullName,
                HeadshotUrl = user.HeadshotUrl ?? ""
            };
        }
    }
}
