namespace EduCore.Business.DTOs;

public sealed record CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int Quota { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public string InstructorId { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
