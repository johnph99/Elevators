using SwitchBoard.Extensions;
using SwitchBoard.Objects;
using System.ComponentModel;
using System.Drawing;

namespace SwitchBoard
{
    public class Control// : IControl /*: IDisposable*/
    {
        private bool _disposed;

        private List<Elevator> _elevators = new();
        private int _bottomFloor = 0;
        private int _topFloor = 5;

        private List<WaitingLoad> WaitingLoads = new();

        public int Check { get; set; }
        public void DoCheck()
        {
            Check++;
        }
        public Control()
        {
        }
       
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Stop();
                    _elevators = null;
                }
            }
            //dispose unmanaged resources
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        
        public Elevator AddAlivator(enElivatorType type)
        {
            Elevator newElevator = null;

            switch (type)
            {
                case enElivatorType.Service:
                    newElevator = new ServiceElevator() { Floor = 0, Name = $"E {_elevators.Count() + 1} (Service)" };
                    break;
                case enElivatorType.Glass:
                    newElevator = new GlassElevator() { Floor = 0, Name = $"E {_elevators.Count() + 1} (Glass)" };
                    break;
                default:
                    newElevator = new StandardElevator() { Floor = 0, Name = $"E {_elevators.Count() + 1} (Standard)" };
                    break;

            }
            _elevators.Add(newElevator);
            return newElevator;
        }

        private bool _enabled = false;

        public async Task Start(int noOfFloors=5, int noOfBAsements=0)
        {
            _bottomFloor = 0 - noOfBAsements;
            _topFloor = noOfFloors;

            _enabled = true;

            Task.Run(() => MoveElevators());
            
        }

        public void Stop()
        {
            _enabled = false; 
        }
      
        private async void MoveElevators()
        {
            while (_enabled) 
            {
                //move elevators
                foreach (var elevator in _elevators)
                {
                    MoveElevator(elevator);
                    //wait 1/10 seconds
                    await Task.Delay(500/_elevators.Count);
                }
                CheckWaintLoads();

               
            }
        }

        private void CheckWaintLoads()
        {
            if(WaitingLoads.Any())
            {
                var wLoad = WaitingLoads.First();
                CallElevator(wLoad.FloorNumber, wLoad.Load, wLoad.DestinationFloor, wLoad.Type);
                WaitingLoads.Remove(wLoad);
            }
        }

        private async void MoveElevator(Elevator elevator)
        {
            var nextFloor = elevator.StopFloors.FirstOrDefault();
            if (nextFloor==null)
            {
                return;
            }
            if (elevator.Status == enStatus.MovingUp)
            {
                elevator.Floor += 0.2;
            }
            else if (elevator.Status == enStatus.MovingDown)
            {
                elevator.Floor -= 0.2;
            }
            elevator.Floor = Math.Round(elevator.Floor, 1);
            if (elevator.Floor == nextFloor.Floor)
            {
                //rasie event to indicate floor reached

                //pause for a few seconds to offload

                //offload
                elevator.Load -= nextFloor.Load;

                //remove floor
                elevator.StopFloors.Remove(nextFloor);
                elevator.Status = enStatus.Idle;
            }
            //raise event
            ElevatorEventArgs args = new();
            args.Elevator = elevator;
            args.TimeReached = DateTime.Now;
            OnMoveMentEvent(args);

        }

        public delegate void ElevatorEventHandler(Object sender, ElevatorEventArgs e);

        public event ElevatorEventHandler ElevatorMoved;
        public virtual void OnMoveMentEvent(ElevatorEventArgs e)
        {
           ElevatorMoved?.Invoke(this, e);
        }

        /// <summary>
        /// Call elevators to move all occupants
        /// </summary>
        /// <param name="eType">default = standard, 1 = glass , 2 = service</param>
        /// <param name="currentFloor">the pickup floor</param>
        /// <param name="destinationFloor">the destination floor</param>
        /// <param name="Capacity">numer of people or cargo items to move</param>
        public int CallElevators(enElivatorType type, int currentFloor, int destinationFloor, int load)
        {

            int outstandingLoad = load;
            while (outstandingLoad > 0)
            {
                var remainingLoad = CallElevator(currentFloor, outstandingLoad, destinationFloor, type);
                if (remainingLoad == outstandingLoad)
                {
                    outstandingLoad = remainingLoad;
                    //wait a while and try again, from the calling procedure
                    break; 
                }
                else
                    outstandingLoad = remainingLoad;
            }
            return outstandingLoad;
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
        private int CallElevator(int floorNumber, int load, int destinationFloor, enElivatorType type)
        {
            if (destinationFloor<_bottomFloor)
                destinationFloor = _bottomFloor;
            else if (destinationFloor>_topFloor)
                destinationFloor = _topFloor;
            
            try
            {
                List<Elevator> availableElevators;
                List<Elevator> selectedElevators;

                switch (type)
                {
                    case enElivatorType.Glass:
                        availableElevators = _elevators.Where(ev => ev.GetType() == typeof(GlassElevator)).ToList();
                        break;
                    case enElivatorType.Service:
                        availableElevators = _elevators.Where(ev => ev.GetType() == typeof(ServiceElevator)).ToList();
                        break;
                    default:
                        availableElevators = _elevators.Where(ev => ev.GetType() == typeof(StandardElevator)).ToList();
                        break;
                }
                //find all elevators going in the directon or idle
                if (destinationFloor > floorNumber)
                {
                    //moving up
                    selectedElevators = availableElevators.Where(ev => (ev.Status == enStatus.MovingUp || ev.Status == enStatus.Idle) && ev.Floor <= floorNumber).ToList();
                }
                else
                {
                    //moving down
                    selectedElevators = availableElevators.Where(ev => (ev.Status == enStatus.MovingDown || ev.Status == enStatus.Idle) && ev.Floor>= floorNumber).ToList();
                }
                if (!selectedElevators.Any())
                {
                    //move the closet elevator to the floor;
                    if (availableElevators.Any())
                    {
                        var firstElevator = availableElevators.First();
                        if (firstElevator.StopFloors.FirstOrDefault(a => a.Floor == destinationFloor) != null)
                        {
                            return load;
                        }
                        int f = Convert.ToInt32(firstElevator.Floor);
                        var rsult = CallElevator(f, 0, floorNumber, type);
                        WaitingLoads.Add((new WaitingLoad() { FloorNumber = floorNumber, Load = load, DestinationFloor = destinationFloor, Type = type }));
                        return load;
                    } 
                }

                //order by closesness
                selectedElevators = selectedElevators.OrderBy(ev => ev.Distance(floorNumber)).ToList();
                //take the first one
                int outstandingCapacity = load;
                int elevatorIndex = 0;
                bool moveEmpty = outstandingCapacity == 0;

                while (outstandingCapacity > 0 || moveEmpty)
                {
                    if (moveEmpty == false) 
                    {
                        int a = 1;
                    }
                    else
                    moveEmpty = false;
                    //check for loading/offloading before floor is reached

                    if (selectedElevators[elevatorIndex].Load < selectedElevators[elevatorIndex].Capacity) //avoid overloading
                    {
                        int numerOfOccupants = 0;
                        if (selectedElevators[elevatorIndex].Capacity> outstandingCapacity)
                        {
                            numerOfOccupants = outstandingCapacity;
                            outstandingCapacity = 0;
                        }
                        else
                        {
                            numerOfOccupants = selectedElevators[elevatorIndex].Capacity - selectedElevators[elevatorIndex].Load;
                            outstandingCapacity -= numerOfOccupants;
                            WaitingLoads.Add((new WaitingLoad() { FloorNumber = floorNumber, Load = outstandingCapacity, DestinationFloor = destinationFloor, Type = type }));
                        }
                        QueElevator(selectedElevators[elevatorIndex], floorNumber, destinationFloor, numerOfOccupants);
                        elevatorIndex++;

                        //check if we ran out of elevators
                        if (elevatorIndex == selectedElevators.Count)
                            break;
                    }
                    else
                    {
                        var outstanding = selectedElevators[elevatorIndex].Capacity - selectedElevators[elevatorIndex].Load;
                        WaitingLoads.Add((new WaitingLoad() { FloorNumber = floorNumber, Load = outstanding, DestinationFloor = destinationFloor, Type = type }));
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

        private void QueElevator(Elevator elevator, int currentFloor, int destinationFloor, int load)
        {
            var newStop = new StopFloor() { Floor = destinationFloor, Load = load };
            if (elevator.StopFloors.Any(a=>a.Floor==newStop.Floor && a.Load==newStop.Load)) 
            {
                return;
            }
            StopFloor nextfloor;
            if (currentFloor < destinationFloor) 
            { 
                //move up
                nextfloor = elevator.StopFloors.FirstOrDefault(a=>a.Floor>destinationFloor);
                elevator.Status=enStatus.MovingUp;
            }
            else
            {
                //movedown
                nextfloor = elevator.StopFloors.FirstOrDefault(a => a.Floor < destinationFloor);
                elevator.Status = enStatus.MovingDown;
            }
            if (nextfloor == null) 
            {
                elevator.StopFloors.Add(newStop);
            }
            else
            {
                int index = elevator.StopFloors.IndexOf(nextfloor);
                elevator.StopFloors.Insert(index - 1, newStop);
            }
            elevator.Load = load;
        }

    }
}
