using EduCore.Business.DTOs;
using EduCore.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public InstructorsController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetInstructors()
        {
            var instructors = await _userManager.GetUsersInRoleAsync("Instructor");
            
            var result = instructors
                .Where(u => u.IsApproved && u.IsActive)
                .Select(u => new InstructorDto
                {
                    Id = u.Id,
                    FullName = u.FullName ?? u.UserName ?? "Eğitmen",
                    Username = u.UserName ?? "",
                    Bio = u.Bio,
                    HeadshotUrl = u.HeadshotUrl
                });

            return Ok(result);
        }
    }
}
