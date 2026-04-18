using EduCore.Business.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduCore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "SuperAdmin")] // Sadece SuperAdmin erişebilir
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("pending-instructors")]
    public async Task<IActionResult> GetPendingInstructors()
    {
        // Artık servis içeride AutoMapper kullanarak AuthResponseDto listesi dönüyor
        var instructors = await _adminService.GetPendingInstructorsAsync();
        return Ok(instructors);
    }

    [HttpPost("approve-instructor/{username}")]
    public async Task<IActionResult> ApproveInstructor(string username)
    {
        try
        {
            await _adminService.ApproveInstructorAsync(username);
            return Ok(new { Message = "Eğitmen başarıyla onaylandı." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("pending-courses")]
    public async Task<IActionResult> GetPendingCourses()
    {
        // Servis katmanında Course -> CourseDto mapping'i yapıldı
        var courses = await _adminService.GetPendingCoursesAsync();
        return Ok(courses);
    }

    [HttpPost("approve-course/{courseId}")]
    public async Task<IActionResult> ApproveCourse(int courseId)
    {
        try
        {
            await _adminService.ApproveCourseAsync(courseId);
            return Ok(new { Message = "Kurs başarıyla onaylandı ve yayına alındı." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("all-courses")]
    public async Task<IActionResult> GetAllCourses()
    {
        // AdminCourseDto listesi döner (kazanç bilgileri dahil)
        var courses = await _adminService.GetAllCoursesAdminAsync();
        return Ok(courses);
    }

    [HttpGet("all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        // AdminUserDto listesi döner
        var users = await _adminService.GetAllUsersAdminAsync();
        return Ok(users);
    }

    [HttpPost("toggle-course/{courseId}")]
    public async Task<IActionResult> ToggleCourse(int courseId)
    {
        try
        {
            await _adminService.ToggleCourseActiveAsync(courseId);
            return Ok(new { Message = "Kursun aktiflik durumu değiştirildi." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("toggle-user/{userId}")]
    public async Task<IActionResult> ToggleUser(string userId)
    {
        try
        {
            await _adminService.ToggleUserActiveAsync(userId);
            return Ok(new { Message = "Kullanıcının aktiflik durumu değiştirildi." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("instructor/{username}")]
    public async Task<IActionResult> DeleteInstructor(string username)
    {
        try
        {
            await _adminService.DeleteInstructorAsync(username);
            return Ok(new { Message = "Eğitmen ve ilgili tüm kursları başarıyla silindi." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("course/{courseId}")]
    public async Task<IActionResult> DeleteCourse(int courseId)
    {
        try
        {
            await _adminService.DeleteCourseAsync(courseId);
            return Ok(new { Message = "Kurs ve ilgili tüm kayıtlar başarıyla silindi." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}