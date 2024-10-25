using System;
using System.Collections.Generic;

namespace Lib.Models
{
    public partial class BillDetail
    {
        public int BillId { get; set; }
        public int CourseId { get; set; }

        public virtual Bill Bill { get; set; } = null!;
        public virtual Course Course { get; set; } = null!;
    }
}
