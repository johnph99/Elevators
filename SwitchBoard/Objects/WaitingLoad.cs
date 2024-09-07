using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchBoard.Objects
{
    public class WaitingLoad
    {
        public int FloorNumber{get;set;}
        public int Load{get;set;}
        public int DestinationFloor{get;set;}
        public enElivatorType Type { get; set; }
    }
}
