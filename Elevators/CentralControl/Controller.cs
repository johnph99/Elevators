using Elevators.Elevators;
using Elevators.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevators.CentralControl
{
    public class Controller
    {

        public List<Elevator> Elevators { get; set; }

        /// <summary>
        /// Call elevators to move all occupants
        /// </summary>
        /// <param name="eType">default = standard, 1 = glass , 2 = service</param>
        /// <param name="currentFloor">the pickup floor</param>
        /// <param name="destinationFloor">the destination floor</param>
        /// <param name="Capacity">numer of people or cargo items to move</param>
        public void CallElevators(int eType, int currentFloor, int destinationFloor, int Capacity)
        {
            int outstandingLoad = Capacity;
            while (outstandingLoad > 0)
            {
                outstandingLoad = CallElevator(eType, currentFloor, destinationFloor, Capacity);

            }
        }

        /// <summary>
        /// Calls an elevator
        /// if the call returns a positive number, then wait 5 seconds and call again with the outstanding number
        /// </summary>
        /// <param name="eType">default = standard, 1 = glass , 2 = service</param>
        /// <param name="currentFloor">the pickup floor</param>
        /// <param name="destinationFloor">the destination floor</param>
        /// <param name="Capacity">numer of people or cargo items to move</param>
        /// <returns>return the number of occupants not transported</returns>
        private int CallElevator(int eType, int currentFloor, int destinationFloor, int Capacity)
        {
            try
            {
                List<Elevator> availableElevators;
                List<Elevator> selectedElevators ;

                switch (eType)
                {
                    case 1:
                        availableElevators = Elevators.Where(ev => ev.GetType() == typeof(GlassElevator)).ToList();
                        break;
                    case 2:
                        availableElevators = Elevators.Where(ev => ev.GetType() == typeof(ServiceElevator)).ToList();
                        break;
                    default:
                        availableElevators = Elevators.Where(ev => ev.GetType() == typeof(StandardElevator)).ToList();
                        break;
                }
                //find all elevators going in the directon or idle
                if (destinationFloor > currentFloor)
                {
                    //moving up
                    selectedElevators = availableElevators.Where(ev => ev.Status == enStatus.MovingUp || ev.Status == enStatus.Idle).ToList();
                }
                else
                {
                    //moving down
                    selectedElevators = availableElevators.Where(ev => ev.Status == enStatus.MovingDown || ev.Status == enStatus.Idle).ToList();
                }
                if (!availableElevators.Any()) 
                {
                    selectedElevators = availableElevators;
                }

                //order by closesness
                selectedElevators = selectedElevators.OrderBy(ev => ev.Distance(currentFloor)).ToList();
                //take the first one
                int outstandingCapacity = Capacity;
                int elevatorIndex = 0;

                while (outstandingCapacity > 0) 
                {
                    if(selectedElevators[elevatorIndex].Occupants >= selectedElevators[elevatorIndex].Capacity) //avoid overloading
                    {
                        int numerOfOccupants = 0;
                        if (selectedElevators[elevatorIndex].Occupants - outstandingCapacity > 0)
                        {
                            numerOfOccupants = outstandingCapacity;
                            outstandingCapacity = 0;
                        }
                        else
                        {
                            numerOfOccupants = selectedElevators[elevatorIndex].Capacity - selectedElevators[elevatorIndex].Occupants;
                            outstandingCapacity -= numerOfOccupants;
                        }
                        QueElevator(selectedElevators[elevatorIndex], currentFloor, destinationFloor, numerOfOccupants);
                        elevatorIndex++;
                        
                        //check if we ran out of elevators
                        if(elevatorIndex==selectedElevators.Count) 
                            break;
                    }
                }

                return outstandingCapacity;
            }
            catch (Exception ex) 
            {
                //log the error to the console
                return 0;
            }
        }

        private void QueElevator(Elevator elevator, int currentFloor, int destinationFloor, int numerOfOccupants)
        {
            throw new NotImplementedException();
        }
    }
}
