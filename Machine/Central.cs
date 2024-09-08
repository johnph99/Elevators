using Machine.Extensions;
using Machine.Objects;
using System.Diagnostics;

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

        List<WaitingLoad> _waitingQue = new();

        private List<Elevator> _elevators = new();
        private int _bottomFloor = 0;
        private int _topFloor = 5;
        private bool _enabled = false;

        public List<Elevator> Elevators { get { return _elevators; } }
        public List<WaitingLoad> WaitQue { get { return _waitingQue; } }

        private async void OpperateLifts()
        {
            while (_enabled)
            {
                //move the elevators
                await MoveElevators();
                await Task.Delay(500);
            }
        }

        private async Task MoveElevators()
        {
            //move elevators
            foreach (var elevator in _elevators)
            {
                await MoveElevator(elevator);
                //wait 1/10 seconds
                await Task.Delay(500 / _elevators.Count);
            }

        }

        private async Task MoveElevator(Elevator elevator)
        {
            //move the elevator

            if (elevator.Status == enStatus.MovingUp)
            {
                elevator.Floor += 0.1;
            }
            else if (elevator.Status == enStatus.MovingDown)
            {
                elevator.Floor -= 0.1;
            }
            elevator.Floor = Math.Round(elevator.Floor, 1);
            await Task.Delay(100);

            //find the closest call for the current elevator
            WaitingLoad waiting = null;
            
            if(elevator.GetType()== typeof(GlassElevator))
            {
                waiting = _waitingQue.FirstOrDefault(a => (a.Direction == elevator.Status || elevator.Status == enStatus.Idle) && a.Type==enElivatorType.Glass);
            }
            else if (elevator.GetType() == typeof(ServiceElevator))
            {
                waiting = _waitingQue.FirstOrDefault(a => (a.Direction == elevator.Status || elevator.Status == enStatus.Idle) && a.Type == enElivatorType.Service);
            }
            else
            {
                waiting = _waitingQue.FirstOrDefault(a => (a.Direction == elevator.Status || elevator.Status == enStatus.Idle) && a.Type == enElivatorType.Standard);
            }

            //check if there are elivators that are closer to the floor
            if (waiting != null)
            {
                Elevator closestElevator = null;
                List<Elevator> availableElevators = null;
                switch (waiting.Type)
                {
                    case enElivatorType.Glass:
                        availableElevators = _elevators.Where(ev => (ev.Status == waiting.Direction || ev.Status == enStatus.Idle) && ev.GetType() == typeof(GlassElevator)).ToList();
                        break;
                    case enElivatorType.Service:
                        availableElevators = _elevators.Where(ev => (ev.Status == waiting.Direction || ev.Status == enStatus.Idle) && ev.GetType() == typeof(ServiceElevator)).ToList();
                        break;
                    default:
                        availableElevators = _elevators.Where(ev => (ev.Status == waiting.Direction || ev.Status == enStatus.Idle) && ev.GetType() == typeof(StandardElevator)).ToList();
                        break;
                }
                closestElevator = availableElevators.OrderBy(a=>a.Distance(waiting.FloorNumber)).FirstOrDefault();

                if (closestElevator != null && closestElevator.Name != elevator.Name)
                {
                    //if there is then just keep moving this elevator
                    //first check if we should unload
                    //check if we should unload before loading
                    if (elevator.Floor == elevator.nextStop)
                    {
                        var offload = elevator.Loads.FirstOrDefault(a => a.DestinationFloor == elevator.Floor);
                        if (offload != null)
                        {
                            //offload
                            await OffLoadElevator(elevator, offload);
                        }
                    }
                }
                else
                {
                    // else add this floor to the elevator if not already added.
                    //if (waiting != null)
                    //{
                    //    elevator.nextStop = waiting.FloorNumber;
                    //}
                    //now check if the elevator is on the floor
                    if (elevator.Floor == elevator.nextStop)
                    {
                        //stop and load / unload
                        elevator.Status = enStatus.Idle;

                        //check if we should unload before loading
                        var offload = elevator.Loads.FirstOrDefault(a => a.DestinationFloor == elevator.Floor);

                        if (offload != null)
                        {
                            //offload
                            await OffLoadElevator(elevator, offload);
                            elevator.nextStop = waiting.FloorNumber;
                            return;
                        }

                        //if loading then start moving the elevator again.
                        //load the elevator
                        if (waiting != null)
                        {
                           
                            var remainingLoad = await SendElevator(elevator, waiting);
                            _waitingQue.Remove(waiting);
                            if (remainingLoad > 0)
                            {
                                RequestElevator(waiting.Type, waiting.FloorNumber, waiting.DestinationFloor, remainingLoad);
                            }
                        }
                    }
                    else
                    {
                        //if not keep moving
                        if (elevator.Status == enStatus.Idle) 
                        {
                            if (elevator.Floor < elevator.nextStop)
                                elevator.Status = enStatus.MovingUp;
                            else if (elevator.Floor>elevator.nextStop)
                                elevator.Status= enStatus.MovingDown;

                        } 
                    }
                }
            }
            else if (elevator.Floor == elevator.nextStop)
            {
                //should we offload?
                var offload = elevator.Loads.FirstOrDefault(a => a.DestinationFloor == elevator.Floor);
                    //offload
                    await OffLoadElevator(elevator, offload);
                //load the elevator

            }
            else
            {
                Debug.WriteLine(elevator.Floor);
                Debug.WriteLine(elevator.nextStop);
            }
            
            if (elevator.Status == enStatus.Idle)
            {
                if (elevator.Loads.Any())
                {
                    if(elevator.Loads.FirstOrDefault().DestinationFloor>elevator.Floor)
                        elevator.Status =enStatus.MovingDown;
                    else if(elevator.Loads.FirstOrDefault().DestinationFloor < elevator.Floor)
                        elevator.Status=enStatus.MovingUp;
                }
            }

            
            //raise event
            ElevatorEventArgs args = new();
            args.Elevator = elevator;
            args.TimeReached = DateTime.Now;
            OnMoveMentEvent(args);

            await Task.Delay(100);
        }

        private async Task OffLoadElevator(Elevator elevator, ElevatorLoad offload)
        {
            var direction = elevator.Status;

            elevator.Status = enStatus.Idle;
            await Task.Delay(100);
            if (offload != null)
            {
                for (var i = 0; i < offload.Load; i++)
                {
                    elevator.Load -= 1;
                    //raise event
                    ElevatorEventArgs args = new();
                    args.Elevator = elevator;
                    args.TimeReached = DateTime.Now;
                    OnMoveMentEvent(args);
                    await Task.Delay(100);
                }

                elevator.Loads.Remove(offload);
            }
            if (elevator.Loads.Any())
                elevator.Status = direction;
        }

        private async Task<int> SendElevator(Elevator elevator, WaitingLoad waiting)
        {
            int actualLoad = 0;
            int remaingLoad = 0;

            if (elevator.Floor == waiting.FloorNumber)
            {
                var availableSpace = elevator.Capacity - elevator.Load;

                actualLoad = waiting.Load;
                if (availableSpace > waiting.Load)
                {
                    for (var i = 0; i < waiting.Load; i++)
                    {
                        elevator.Load += 1;
                        //raise event
                        ElevatorEventArgs args = new();
                        args.Elevator = elevator;
                        args.TimeReached = DateTime.Now;
                        OnMoveMentEvent(args);
                        await Task.Delay(100);
                    }
                }
                else
                {
                    actualLoad = availableSpace;
                    remaingLoad = waiting.Load - actualLoad;
                    elevator.Load = actualLoad;
                }

                //to send the elevator, call it from the desitnation floor
                ElevatorLoad destFloor = new ElevatorLoad();
                destFloor.Load = actualLoad;
                destFloor.DestinationFloor = waiting.DestinationFloor;
                elevator.Loads.Add(destFloor);
                elevator.Status = waiting.Direction;
                elevator.nextStop = waiting.DestinationFloor;
            }
            else
            {
                //to send the elevator, call it from the desitnation floor
                ElevatorLoad destFloor = new ElevatorLoad();
                destFloor.Load = actualLoad;
                destFloor.DestinationFloor = waiting.FloorNumber;
                elevator.Loads.Add(destFloor);
                if(destFloor.DestinationFloor>elevator.Floor)
                    elevator.Status = enStatus.MovingUp;
                else if (destFloor.DestinationFloor<elevator.Floor)
                    elevator.Status= enStatus.MovingDown;
                
                elevator.nextStop = waiting.FloorNumber;

                remaingLoad = waiting.Load;
            }

            return remaingLoad;
        }

        private void CallElevator(Elevator elevator, WaitingLoad forload )
        {
            var newStop = new ElevatorLoad() { Load = 0, DestinationFloor=forload.FloorNumber };
            

            ElevatorLoad nextfloor;
            if (forload.Direction==enStatus.MovingUp)
            {
                //move up
                nextfloor = elevator.Loads.FirstOrDefault(a => a.DestinationFloor > forload.DestinationFloor);
                //elevator.Status = enStatus.MovingUp;
            }
            else
            {
                //movedown
                nextfloor = elevator.Loads.FirstOrDefault(a => a.DestinationFloor < forload.DestinationFloor);
                //elevator.Status = enStatus.MovingDown;
            }
            if (nextfloor == null)
            {
                elevator.Loads.Add(newStop);
            }
            else
            {
                int index = elevator.Loads.IndexOf(nextfloor);
                elevator.Loads.Insert(index - 1, newStop);
            }
            
        }

        ////
        /// Public Methods
        /// 
        public void AddAlivator(enElivatorType type)
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
            //return newElevator;
        }

        public void RequestElevator(enElivatorType type, int currentFloor, int destinationFloor, int load)
        {
            WaitingLoad newLoad = new WaitingLoad() { DestinationFloor = destinationFloor, Load = load, FloorNumber = currentFloor, Type = type };
            _waitingQue.Add(newLoad);
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
