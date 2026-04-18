using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
