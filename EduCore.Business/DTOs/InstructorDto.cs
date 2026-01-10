namespace EduCore.Business.DTOs
{
    public class InstructorDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? HeadshotUrl { get; set; }
    }
}
