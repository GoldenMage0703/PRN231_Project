using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO.Options
{
    public class OptionDTO
    {
        public int Id { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
