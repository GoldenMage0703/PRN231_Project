using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO.Options
{
    public class CreateOptionDTO
    {
        public string OptionText { get; set; } = null!;
        public bool isTrue { get; set; } = false;
    }
}
