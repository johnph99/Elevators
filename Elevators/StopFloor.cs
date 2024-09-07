using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevators
{
    public class StopFloor
    {
        public int Floor { get;set; }
        /// <summary>
        /// Indicates how many occupants must be loaded (+) or unloaded (-1)
        /// </summary>
        public int Occupants { get; set; }
    }
}
