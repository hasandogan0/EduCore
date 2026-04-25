namespace EduCore.Business.DTOs;

public sealed record AuthResponse(
    string Token,
    string Username,
    string Email,
    string Role,
    string FullName,
    string HeadShotUrl
);

