using Elevators.Elevators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevators.Extensions
{
    public static class Extensions
    {
        public static double Distance(this Elevator elevator, int FromFloor)
        {
            return Math.Abs(elevator.Floor - FromFloor);
        }
    }
}
