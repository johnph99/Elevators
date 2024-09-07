using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchBoard.Objects
{
    public class StandardElevator : Elevator
    {
        public override int Capacity { get; } = 10;
    }
}
