using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Business.DTOs
{
    public class AdminUserDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalEarnings { get; set; } // Only relevant for instructors
        public string HeadshotUrl { get; set; }
    }
}
