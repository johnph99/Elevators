using Machine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine.Interfaces
{
    public interface ICentral
    {
        /// <summary>
        /// The list of all the elevators
        /// </summary>
        List<IElevator> Elevators { get; }

        /// <summary>
        /// The elevator request que
        /// </summary>
        List<WaitingLoad> WaitQue { get; }

        /// <summary>
        /// Gets the running status of the Central Controller
        /// </summary>
        bool Running { get; }

        /// <summary>
        /// Adds a new elevator to the system
        /// </summary>
        /// <param name="type">The type of elevator to add</param>
        void AddAlivator(enElevatorType type);

        /// <summary>
        /// Call an Elevator to a specific floor
        /// </summary>
        /// <param name="type">The type of Elevator to call</param>
        /// <param name="fromFloor">the floor the Elevator is required</param>
        /// <param name="destinationFloor">the floor the elevator is required to stop/offload</param>
        /// <param name="load">the number of items/persons to transport</param>
        void RequestElevator(enElevatorType type, int fromFloor, int destinationFloor, int load);

        /// <summary>
        /// Starts the system and runs all Elevators
        /// </summary>
        /// <param name="noOfFloors">The number of floors in the building, default is 5</param>
        /// <param name="noOfBAsements">The number of Basements in the building, default is 0</param>
        /// <returns></returns>
        Task Start(int noOfFloors = 5, int noOfBAsements = 0);

        /// <summary>
        /// stops the system from running
        /// </summary>
        void Stop();

        event ElevatorEventHandler ElevatorMoved;

    }
}
