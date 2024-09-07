using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine.Objects
{
    public class ServiceElevator : Elevator
    {
        public override int Capacity { get; } = 20;
    }
}
