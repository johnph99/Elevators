using Machine.Objects;

namespace Machine
{
    public class Central
    {

        //call elivator by queing the call
        //wait for elevator to arive and the floor
        //then load it
        //if any outstanding loads, call elevator again

        //so we have a list of qued actions
        // this will contain the from floor, to floor, load, elevator type (waiting load)

        //we need to check the que and cross reference it with all the elivatorrs

        List<WaitingLoad> _waiting = new();
        private List<Elevator> _elevators = new();
        private int _bottomFloor = 0;
        private int _topFloor = 5;
        private bool _enabled = false;

        private async void OpperateLifts()
        {
            while (_enabled)
            {

                foreach (var waiting in _waiting)
                {
                    List<Elevator> availableElevators;
                    List<Elevator> selectedElevators;

                    //find the closest lift going in the direction the wating item wants to go
                    switch (waiting.Type)
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
                    selectedElevators = availableElevators.Where(ev => (ev.Status == waiting.Direction || ev.Status == enStatus.Idle)).ToList();
                    if (waiting.Direction == enStatus.MovingDown)
                    {
                        selectedElevators = selectedElevators.Where(a => a.Floor >= waiting.FloorNumber).ToList();
                    }
                    else
                    {
                        selectedElevators = selectedElevators.Where(a => a.Floor <= waiting.FloorNumber).ToList();
                    }

                    if (!selectedElevators.Any())
                    {
                        //call the closet elevator to the floor;
                        var firstElevator = availableElevators.First();
                        if (firstElevator.Floor == waiting.FloorNumber)
                        {
                            //elivator has arrived
                            //remove from que
                            _waiting.Remove(waiting);
                            //que new destination
                            int remaingingLoad = SendElevator(firstElevator, waiting);
                            if (remaingingLoad > 0)
                            {
                                await Task.Delay(100);

                                availableElevators.Remove(firstElevator);
                                firstElevator = availableElevators.First();
                                CallElevator(firstElevator, waiting);
                            }

                        }
                        else if (firstElevator.StopFloors.FirstOrDefault(a => a.Floor == waiting.FloorNumber) == null)
                        {
                            CallElevator(firstElevator, waiting);
                        }
                    }
                    else
                    {
                        var firstElevator = selectedElevators.First();
                        if (firstElevator.Floor == waiting.FloorNumber)
                        {
                            //elivator has arrived
                            //offload the elevator
                            var stopfloor = firstElevator.StopFloors.FirstOrDefault();
                            firstElevator.Load -= stopfloor.Load;
                            firstElevator.StopFloors.Remove(stopfloor);
                            await Task.Delay(500);

                            //remove from que
                            _waiting.Remove(waiting);
                            //que new destination
                            int remaingingLoad = SendElevator(firstElevator, waiting);
                            if (remaingingLoad > 0)
                            {
                                await Task.Delay(100);

                                availableElevators.Remove(firstElevator);
                                firstElevator = availableElevators.First();
                                CallElevator(firstElevator, waiting);
                            }

                        }
                    }
                }

            }
        }

        private int SendElevator(Elevator elevator, WaitingLoad waiting)
        {
            var availableSpace = elevator.Capacity - elevator.Load;
            int remaingLoad = 0;
            if (availableSpace > waiting.Load)
                elevator.Load += waiting.Load;
            else
            {
                remaingLoad = elevator.Capacity - availableSpace;
                elevator.Load = elevator.Capacity;
            }
            //to send the elevator, call it from the desitnation floor
            StopFloor destFloor = new StopFloor();
            destFloor.Load = waiting.Load;
            destFloor.Floor = waiting.DestinationFloor;
            elevator.StopFloors.Add(destFloor);

            return remaingLoad;
        }

        private void CallElevator(Elevator elevator, WaitingLoad forload )
        {
            var newStop = new StopFloor() { Load = 0, Floor=forload.FloorNumber };
            

            StopFloor nextfloor;
            if (forload.Direction==enStatus.MovingUp)
            {
                //move up
                nextfloor = elevator.StopFloors.FirstOrDefault(a => a.Floor > forload.DestinationFloor);
                elevator.Status = enStatus.MovingUp;
            }
            else
            {
                //movedown
                nextfloor = elevator.StopFloors.FirstOrDefault(a => a.Floor < forload.DestinationFloor);
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
            
        }

        ////
        /// Public Methods
        /// 
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

        public void RequestElevator(enElivatorType type, int currentFloor, int destinationFloor, int load)
        {
            WaitingLoad newLoad = new WaitingLoad() { DestinationFloor = destinationFloor, Load = load, FloorNumber = currentFloor, Type = type };
            _waiting.Add(newLoad);
        }

        public async Task Start(int noOfFloors = 5, int noOfBAsements = 0)
        {
            _bottomFloor = 0 - noOfBAsements;
            _topFloor = noOfFloors;

            _enabled = true;

            Task.Run(() => OpperateLifts());

        }

        public void Stop()
        {
            _enabled = false;
        }


        /////////////////////////
        ///EVENTS
        /////////////////////////

        public delegate void ElevatorEventHandler(Object sender, ElevatorEventArgs e);

        public event ElevatorEventHandler ElevatorMoved;
        public virtual void OnMoveMentEvent(ElevatorEventArgs e)
        {
            ElevatorMoved?.Invoke(this, e);
        }
    }
}
