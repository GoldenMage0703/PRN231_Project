﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.DTO
{
    public class CreateCategoryDTO
    {
        public CreateCategoryDTO() { }
      
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
    }
}