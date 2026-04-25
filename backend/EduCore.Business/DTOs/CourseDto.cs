namespace EduCore.Business.DTOs;

public sealed record CourseDto(
    int Id,
    string Title,
    string Description,
    decimal Price,
    string ImageUrl,
    int Quota,
    string Status,
    string CategoryName,
    string InstructorName,
    string InstructorId,
    bool IsActive
    );
