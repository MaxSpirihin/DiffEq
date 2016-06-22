using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DiffEq.Helpers
{
    public class GraphInfo
    {
        public bool IsEquation { get; set; }
        public string Function { get; set; }
        public double x0 { get; set; }
        public double y0 { get; set; }
        public string color { get; set; }
        public int Thinkness { get; set; }
    }
}
