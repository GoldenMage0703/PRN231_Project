using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO
{
    public class GetCourseDTO
    {
        public GetCourseDTO() { }
        public int Id { get; set; }
        public string CourseName { get; set; } = null!;
        public bool Publish { get; set; }
        public int TotalJoined { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte[]? Image { get; set; }
        public decimal? Price { get; set; }
        public string CategoryName { get; set; }
        public int Status { get; set; }
    }
}
