namespace EduCore.Business.DTOs;

public sealed record AdminCourseDto(
    int Id,
    string Title,
    string Instructor,
    decimal Price,
    string Status,
    bool IsActive,
    decimal TotalEarnings
);

