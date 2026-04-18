using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Business.DTOs;

public class CreateCourseDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int Quota { get; set; }
    public int CategoryId { get; set; }
}


// En son UpdateCourseDto kısmında kaldık. Buna göre devam edeceğiz.