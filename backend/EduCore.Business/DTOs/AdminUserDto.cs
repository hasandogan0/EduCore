namespace EduCore.Business.DTOs;

public sealed record AdminUserDto(
    int Id,
    string Username,
    string Fullname,
    string Email,
    string Role,
    bool IsActive,
    decimal TotalEarnings,
    string HeadShotUrl
    );
