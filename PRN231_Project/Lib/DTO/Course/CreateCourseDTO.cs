using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO.Course
{
    public class CreateCourseDTO
    {
        public CreateCourseDTO() { }
        public string CourseName { get; set; } = null!;
        public bool Publish { get; set; }
        public int TotalJoined { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal? Price {  get; set; }
        public byte[]? Image { get; set; }
        public int Category { get; set; }
    }
}
