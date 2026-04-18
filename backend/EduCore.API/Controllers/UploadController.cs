using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduCore.API.Controllers
{
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
                return BadRequest("No file uploaded or file is empty.");

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return BadRequest($"Invalid file type. Allowed: {string.Join(", ", allowedExtensions)}");

            try {
                // Create uploads folder if not exists
                // Use ContentRootPath if WebRootPath is null (fallback)
                var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                
                // Ensure wwwroot exists
                if(!Directory.Exists(webRootPath)) Directory.CreateDirectory(webRootPath);

                var uploadsFolder = Path.Combine(webRootPath, "uploads", subFolder);
                
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename
                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var url = $"/uploads/{subFolder}/{fileName}";
                return Ok(new { Url = url });
            } catch(Exception ex) {
                return StatusCode(500, $"Internal server error: {ex.Message} | Path: {Directory.GetCurrentDirectory()}");
            }
        }
    }
}
