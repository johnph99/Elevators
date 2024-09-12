using Machine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine.Interfaces
{
    public interface IElevator
    {
        /// <summary>
        /// Elevator name, used for identification
        /// </summary>
        string Name { get; set; }
        
        /// <summary>
        /// Indicates the max number of persons or items that can be loaded into the elevator
        /// </summary>
        abstract int Capacity { get; }
        
        /// <summary>
        /// Get or set the current number of occupants
        /// </summary>
        int Load { get; set; }
        
        /// <summary>
        /// Indicates the current floor, if the number is between 2 integer values, the elevator is between floors
        /// </summary>
        double Floor { get; set; }

        /// <summary>
        /// Indicates the movement direcotion or status of the elevator
        /// </summary>
        enStatus Direction { get; set; }
        
        /// <summary>
        /// Contains a list of floors the elevator should stop at
        /// </summary>
        List<ElevatorLoad> Loads { get; }

        /// <summary>
        /// get or set the top floor for the elevator
        /// </summary>
        int TopFloor { get; set; }

        /// <summary>
        /// indeicatesd the bottom floor for the elevator
        /// </summary>
        int BottomFloor { get; set; }

        /// <summary>
        /// Moves the elevator by a fraction between levels
        /// </summary>
        void Move();

        /// <summary>
        /// checks ahead for availabe loads
        /// </summary>
        /// <param name="waitingLoads">The list of all Loads that are available</param>
        void FindNextLoad(List<WaitingLoad> waitingLoads);

        /// <summary>
        /// Loads and unloads the loads of the elevator on a floor
        /// </summary>
        /// <param name="waitingLoads">The list of all Loads that are available</param>
        void LoadUnload(List<WaitingLoad> waitingLoads);

        /// <summary>
        /// Enables the elevator to opperate (this is set by default)
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the elevator from opperating
        /// </summary>
        void Stop();

        /////////////////////////
        ///EVENTS
        /////////////////////////

        event ElevatorEventHandler ElevatorMoved;
    }
}
