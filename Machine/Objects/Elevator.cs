using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Machine.Objects
{
    public abstract class Elevator
    {
        public string Name { get; set; }
        /// <summary>
        /// Indicates the max number of persons or items that can be loaded into the elevator
        /// </summary>
        public abstract int Capacity { get; }
        /// <summary>
        /// Get or set the current number of occupants
        /// </summary>
        public int Load { get; set; }
        /// <summary>
        /// Indicates the current floor, if the number is between 2 integer values, the elevator is between floors
        /// </summary>
        public double Floor { get; set; } = 0;
        public enStatus Status { get; set; } = enStatus.Idle;
        /// <summary>
        /// Contains a list of floors the elevator should stop at
        /// </summary>
        public List<StopFloor> StopFloors { get; } = new();
    }
}
