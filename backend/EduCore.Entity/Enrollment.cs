using System.ComponentModel.DataAnnotations.Schema;

namespace EduCore.Entity;

public class Enrollment
{
    public int Id { get; set; }
    
    public string StudentId { get; set; } = string.Empty;
    [ForeignKey("StudentId")]
    public ApplicationUser? Student { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public int ProgressPercentage { get; set; } = 0;
}
