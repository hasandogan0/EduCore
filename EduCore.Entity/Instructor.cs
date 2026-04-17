namespace EduCore.Entity;

public class Instructor
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public string? Bio { get; set; }
    public string? HeadshotUrl { get; set; }
    public ICollection<Course>? Courses { get; set; }
}
