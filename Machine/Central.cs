using Machine.Extensions;
using Machine.Objects;
using System.ComponentModel.Design;
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
                elevator.FindNextLoad(WaitQue);
                elevator.Move();
                elevator.LoadUnload(WaitQue);
                await Task.Delay(500 / _elevators.Count);

            }

        }

        void Elevator_ElevatorMoved(Object sender, ElevatorEventArgs e)
        {
            //raise event

            OnMoveMentEvent(e);
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
                    newElevator = new ServiceElevator() { Floor = 0, Name = $"E {_elevators.Count() + 1} (Service)", TopFloor = _topFloor, BottomFloor = _bottomFloor };
                    break;
                case enElivatorType.Glass:
                    newElevator = new GlassElevator() { Floor = 0, Name = $"E {_elevators.Count() + 1} (Glass)", TopFloor = _topFloor, BottomFloor = _bottomFloor };
                    break;
                default:
                    newElevator = new StandardElevator() { Floor = 0, Name = $"E {_elevators.Count() + 1} (Standard)", TopFloor = _topFloor, BottomFloor = _bottomFloor };
                    break;

            }

            newElevator.ElevatorMoved += Elevator_ElevatorMoved;

            _elevators.Add(newElevator);
            //return newElevator;
        }

        public void RequestElevator(enElivatorType type, int fromFloor, int destinationFloor, int load)
        {
            WaitingLoad newLoad = new WaitingLoad() { DestinationFloor = destinationFloor, Load = load, FloorNumber = fromFloor, Type = type };
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
