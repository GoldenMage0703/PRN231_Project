using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO.Options
{
    public class UpdateOptionDTO
    {
        public int id { get; set; } 
        public string OptionText { get; set; } = null!;
        public bool isCorrect { get; set; } = false;
    }
}
