using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//this is not the right why to do it, is causes data duplication

namespace Machine.Objects
{
    public class Floor
    {
        public int Number { get; set; }
        public List<Elevator> Elevators { get; set; } = new List<Elevator>();
    }
}
