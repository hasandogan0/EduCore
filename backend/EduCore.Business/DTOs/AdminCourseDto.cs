namespace EduCore.Business.DTOs;

public sealed record AdminCourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public decimal TotalEarnings { get; set; }
}
