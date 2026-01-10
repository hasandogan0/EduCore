using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCore.Entity
{
    public class Lesson
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? VideoUrl { get; set; } // Path to uploaded video
        
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
    }
}
