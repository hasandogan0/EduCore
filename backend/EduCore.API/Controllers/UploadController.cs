using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduCore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public UploadController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost("image")]
    [Authorize(Roles = "Instructor,SuperAdmin")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        // İzin verilen formatları dizi olarak gönderiyoruz
        return await UploadFile(file, new[] { ".jpg", ".jpeg", ".png", ".gif" }, "images");
    }

    [HttpPost("video")]
    [Authorize(Roles = "Instructor,SuperAdmin")]
    [RequestSizeLimit(100_000_000)] // 100MB limit
    public async Task<IActionResult> UploadVideo(IFormFile file)
    {
        return await UploadFile(file, new[] { ".mp4", ".mkv", ".webm" }, "videos");
    }

    private async Task<IActionResult> UploadFile(IFormFile file, string[] allowedExtensions, string subFolder)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { Message = "Dosya yüklenmedi veya dosya boş." });

        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            return BadRequest(new { Message = $"Geçersiz dosya tipi. İzin verilenler: {string.Join(", ", allowedExtensions)}" });

        try
        {
            // wwwroot dizinini belirle
            var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            // wwwroot yoksa oluştur
            if (!Directory.Exists(webRootPath)) Directory.CreateDirectory(webRootPath);

            // Klasör yapısını oluştur: wwwroot/uploads/images veya videos
            var uploadsFolder = Path.Combine(webRootPath, "uploads", subFolder);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Benzersiz bir dosya adı oluştur (GUID)
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Frontend'e dönülecek URL formatı
            var url = $"/uploads/{subFolder}/{fileName}";
            return Ok(new { Url = url, Message = "Dosya başarıyla yüklendi." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Dosya kaydedilirken sunucu hatası oluştu.", Error = ex.Message });
        }
    }
}