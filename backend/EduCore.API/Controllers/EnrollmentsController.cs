using EduCore.Business.DTOs;
using EduCore.Business.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduCore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
        // Diğer gereksiz UserManager ve CategoryRepository bağımlılıklarını sildik
    }

    [HttpPost("{courseId}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Enroll(int courseId)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        try
        {
            await _enrollmentService.EnrollStudentAsync(courseId, studentId);
            return Ok(new { Message = "Kursa başarıyla kayıt olundu." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("check/{courseId}")]
    [Authorize]
    public async Task<IActionResult> IsEnrolled(int courseId)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        var result = await _enrollmentService.IsStudentEnrolledAsync(courseId, studentId);
        return Ok(result);
    }

    [HttpGet("my-courses")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyCourses()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        // Servis katmanında yaptığımız değişiklik sayesinde artık doğrudan DTO listesi geliyor.
        // Controller içinde manuel foreach/mapping yapmamıza gerek kalmadı.
        var courses = await _enrollmentService.GetStudentEnrollmentsAsync(studentId);

        return Ok(courses);
    }

    [HttpPost("progress/{courseId}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> UpdateProgress(int courseId, [FromBody] int progress)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        await _enrollmentService.UpdateProgressAsync(courseId, studentId, progress);
        return Ok(new { Message = "İlerleme kaydedildi." });
    }

    [HttpGet("progress/{courseId}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetProgress(int courseId)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        var progress = await _enrollmentService.GetEnrollmentProgressAsync(courseId, studentId);
        return Ok(progress);
    }
}