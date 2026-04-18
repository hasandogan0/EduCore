using AutoMapper;
using EduCore.Business.DTOs;
using EduCore.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduCore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InstructorsController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper; // Mapper eklendi

    public InstructorsController(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetInstructors()
    {
        // Rolü "Instructor" olan kullanıcıları getir
        var instructors = await _userManager.GetUsersInRoleAsync("Instructor");

        // Onaylı ve aktif olanları filtrele
        var activeInstructors = instructors.Where(u => u.IsApproved && u.IsActive);

        // Manuel new InstructorDto demek yerine AutoMapper kullanıyoruz
        var result = _mapper.Map<IEnumerable<InstructorDto>>(activeInstructors);

        return Ok(result);
    }
}