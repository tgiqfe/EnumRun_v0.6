using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumRun
{
    public class Range
    {
        public string Name { get; set; }
        public int StartNumber { get; set; }
        public int EndNumber { get; set; }

        public Range() { }

        public override string ToString()
        {
            return string.Format("{0}[{1}-{2}]", this.Name, this.StartNumber, this.EndNumber);
        }
    }
}
