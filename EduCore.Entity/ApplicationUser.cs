using Microsoft.AspNetCore.Identity;

namespace EduCore.Entity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public bool IsApproved { get; set; } = true; // Default to true (for Students), Instructors will be set to false
    public string? Bio { get; set; }

    public string? HeadshotUrl { get; set; }
    public bool IsActive { get; set; } = true;
}
