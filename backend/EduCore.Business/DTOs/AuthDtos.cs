using System.ComponentModel.DataAnnotations;

namespace EduCore.Business.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Student|Instructor)$", ErrorMessage = "Role must be either 'Student' or 'Instructor'")]
        public string Role { get; set; } = "Student";

        public Microsoft.AspNetCore.Http.IFormFile? ProfileImage { get; set; }
    }

    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string HeadshotUrl { get; set; } = string.Empty;
    }
}
