using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevators.Elevators
{
    public abstract class Elevator
    {
        //the name and number of the elevator
        public string Name { get; }
        /// <summary>
        /// Indicates the max number of persons or items that can be loaded into the elevator
        /// </summary>
        public abstract int Capacity { get; }
        /// <summary>
        /// Get or set the current number of occupants
        /// </summary>
        public int Occupants { get; set; }
        /// <summary>
        /// Indicates the current floor, if the number is between 2 integer values, the elevator is between floors
        /// </summary>
        public double Floor { get; set; } = 0;
        public enStatus Status { get; set; } = enStatus.Idle;
        /// <summary>
        /// Contains a list of floors the elevator should stop at
        /// </summary>
        public List<StopFloor> StopFloors { get; } = new();

        public void Move(int toFloor)
        {
            //find the first floor to move to
            while (StopFloors.Any())
            {
                var nextFloor = StopFloors.FirstOrDefault();
                if (nextFloor.Floor > Floor)
                {
                    //Move Down
                    Floor -= 0.2;
                }
                else
                {
                    //move up
                    Floor -= 0.2;
                }
                //pause for a second

                if (Floor == nextFloor.Floor)
                {
                    //rasie event to indicate floor reached

                    //pause for a few seconds to offload

                    //offload
                    Occupants -= nextFloor.Occupants;

                    //remove floor
                    StopFloors.Remove(nextFloor);
                }
            }
        }
    }
}
