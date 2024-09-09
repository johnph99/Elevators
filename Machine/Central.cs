using Machine.Extensions;
using Machine.Interfaces;
using Machine.Objects;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace Machine
{
    public class Central : ICentral
    {

        //call Elevator by queing the call
        //wait for elevator to arive and the floor
        //then load it
        //if any outstanding loads, call elevator again

        //so we have a list of qued actions
        // this will contain the from floor, to floor, load, elevator type (waiting load)

        //we need to check the que and cross reference it with all the Elevatorrs

        List<WaitingLoad> _waitingQue = new();

        private List<IElevator> _elevators = new();
        private int _bottomFloor = 0;
        private int _topFloor = 5;
        private bool _enabled = false;

        /// <summary>
        /// Gets the running status of the Central Controller
        /// </summary>
        public bool Running { get { return _enabled; } }

        /// <summary>
        /// The list of all the elevators
        /// </summary>
        public List<IElevator> Elevators { get { return _elevators; } }

        /// <summary>
        /// The elevator request que
        /// </summary>
        public List<WaitingLoad> WaitQue { get { return _waitingQue; } }

        private async void OpperateLifts()
        {
            try
            {
                while (_enabled)
                {
                    //move the elevators
                    foreach (var elevator in _elevators)
                    {
                        try
                        {
                            elevator.FindNextLoad(WaitQue);
                            elevator.Move();
                            elevator.LoadUnload(WaitQue);
                            await Task.Delay(500 / _elevators.Count);
                        }
                        catch
                        {
                            //break the elevator
                            elevator.Stop();
                        }
                    }
                    // await Task.Delay(500);
                }
            }
            catch (Exception ex)
            {
                //catastrofic error
                _enabled = false;
                //log exception
                return;
            }
        }

        /// <summary>
        /// Adds a new elevator to the system
        /// </summary>
        /// <param name="type">The type of elevator to add</param>
        public void AddAlivator(enElevatorType type)
        {
            try
            {
                Elevator newElevator = null;

                switch (type)
                {
                    case enElevatorType.Service:
                        newElevator = new ServiceElevator() { Floor = 0, Name = $"E {_elevators.Count() + 1} (Service)", TopFloor = _topFloor, BottomFloor = _bottomFloor };
                        break;
                    case enElevatorType.Glass:
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
            catch (Exception ex)
            {
                //log exception
                return;
            }
        }

        /// <summary>
        /// Call an Elevator to a specific floor
        /// </summary>
        /// <param name="type">The type of Elevator to call</param>
        /// <param name="fromFloor">the floor the Elevator is required</param>
        /// <param name="destinationFloor">the floor the elevator is required to stop/offload</param>
        /// <param name="load">the number of items/persons to transport</param>
        public void RequestElevator(enElevatorType type, int fromFloor, int destinationFloor, int load)
        {
            WaitingLoad newLoad = new WaitingLoad() { DestinationFloor = destinationFloor, Load = load, FloorNumber = fromFloor, Type = type };
            _waitingQue.Add(newLoad);
        }

        /// <summary>
        /// Starts the system and runs all Elevators
        /// </summary>
        /// <param name="noOfFloors">The number of floors in the building, default is 5</param>
        /// <param name="noOfBAsements">The number of Basements in the building, default is 0</param>
        /// <returns></returns>
        public async Task Start(int noOfFloors = 5, int noOfBAsements = 0)
        {
            _bottomFloor = 0 - noOfBAsements;
            _topFloor = noOfFloors;

            _enabled = true;

            Task.Run(() => OpperateLifts());

        }

        /// <summary>
        /// stops the system from running
        /// </summary>
        public void Stop()
        {
            _enabled = false;
        }

       
        /////////////////////////
        ///EVENTS
        /////////////////////////

        public event ElevatorEventHandler ElevatorMoved;

        public virtual void OnMoveMentEvent(ElevatorEventArgs e)
        {
            ElevatorMoved?.Invoke(this, e);
        }

        private void Elevator_ElevatorMoved(Object sender, ElevatorEventArgs e)
        {
            //raise event

            OnMoveMentEvent(e);
        }


    }
}
