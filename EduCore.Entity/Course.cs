using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCore.Entity;

public enum CourseStatus
{
    Draft,
    Pending,
    Published
}

public class Course
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    
    public string? ImageUrl { get; set; }
    public int Quota { get; set; } // Max students
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public bool IsActive { get; set; } = true;
    
    // Link to Identity User (Instructor)
    public required string InstructorId { get; set; }
    
    [ForeignKey("InstructorId")]
    public ApplicationUser? Instructor { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<Enrollment>? Enrollments { get; set; }
    public ICollection<Lesson>? Lessons { get; set; }
}
