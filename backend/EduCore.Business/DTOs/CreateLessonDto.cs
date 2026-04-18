using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Business.DTOs
{
    public class CreateLessonDto
    {
        public string Title { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public int CourseId { get; set; }
    }
}
