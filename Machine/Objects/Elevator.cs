using Machine.Extensions;
using Machine.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Machine.Objects
{
    public abstract class Elevator: IElevator
    {
        /// <summary>
        /// Elevator name, used for identification
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Indicates the max number of persons or items that can be loaded into the elevator
        /// </summary>
        public abstract int Capacity { get; }
        
        /// <summary>
        /// Get or set the current number of occupants/items
        /// </summary>
        public int Load { get; set; }
        
        /// <summary>
        /// Indicates the current floor, if the number is between 2 integer values, the elevator is between floors
        /// </summary>
        public double Floor { get; set; } = 0;
        
        /// <summary>
        /// Indicates the movement direcotion or status of the elevator
        /// </summary>
        public enStatus Direction { get; set; } = enStatus.Idle;
        
        /// <summary>
        /// Contains a list of floors the elevator should stop at
        /// </summary>
        public List<ElevatorLoad> Loads { get; } = new();
        
        /// <summary>
        /// get or set the top floor for the elevator
        /// </summary>
        public int TopFloor { get; set; }

        /// <summary>
        /// indeicatesd the bottom floor for the elevator
        /// </summary>
        public int BottomFloor { get; set; }

        /// <summary>
        /// Moves the elevator by a fraction between levels
        /// </summary>
        public async void Move()
        {
            if (Direction == enStatus.OutOfOrder)
            {
                return;
            }
            //find the first floor to move to
            switch (this.Direction)
            {
                case enStatus.MovingDown:
                    Floor -= 0.1;
                    break;
                case enStatus.MovingUp:
                    Floor += 0.1;
                    break;
            }
            Floor = Math.Round(Floor, 1);

            if (Floor >= TopFloor)
            {
                Floor = TopFloor;
                Direction = enStatus.Idle;
            }
            if (Floor <= BottomFloor)
            {
                Floor = BottomFloor;
                Direction = enStatus.Idle;
            }

            //raise event
            ElevatorEventArgs args = new();
            args.Elevator = this;
            args.TimeReached = DateTime.Now;
            OnMoveMentEvent(args);

            await Task.Delay(100);

            ////check if we reached a floor number
            //if (Direction != enStatus.Idle && Floor == Math.Round(Floor, 0))
            //{
            //    //check if we must unload and load
            //    //or maybe do this from another call
            //}
        }

        /// <summary>
        /// checks ahead for availabe loads
        /// </summary>
        /// <param name="waitingLoads"></param>
        public async void FindNextLoad(List<WaitingLoad> waitingLoads)
        {
            try
            {
                if (Direction == enStatus.OutOfOrder)
                {
                    return;
                }

                if (Direction == enStatus.Idle)
                {
                    var elType = this.GetType().ToString();

                    if (waitingLoads.Any())
                    {
                        //move the elevator to the first item
                        var firstWaiting = waitingLoads.OrderBy(a => a.Distance(Floor)).FirstOrDefault(a => elType.Contains(a.Type.ToString()));
                        if (firstWaiting != null)
                        {
                            if (Floor > firstWaiting.FloorNumber)
                                Direction = enStatus.MovingDown;
                            else if (Floor < firstWaiting.FloorNumber)
                                Direction = enStatus.MovingUp;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //log exception
                return;
            }
        }

        /// <summary>
        /// Loads and unloads the loads of the elevator on a floor
        /// </summary>
        /// <param name="waitingLoads"></param>
        public async void LoadUnload(List<WaitingLoad> waitingLoads)
        {
            try
            {
                if (Direction == enStatus.OutOfOrder)
                {
                    return;
                }

                ElevatorEventArgs args = new();
                args = new();
                args.Elevator = this;

                var lastDirection = Direction;

                //check if we need to offload anything on the current floor
                for (int x = Loads.Count - 1; x >= 0; x--)
                {
                    var load = Loads[x];
                    if (load.DestinationFloor == Floor)
                    {
                        Direction = enStatus.Idle;
                        //offload
                        for (var i = 0; i < load.Load; i++)
                        {
                            Load -= 1;
                            //raise event
                            args.TimeReached = DateTime.Now;
                            OnMoveMentEvent(args);
                            await Task.Delay(100);
                            if (Load <= 0)
                            {
                                Load = 0;
                                break;
                            }
                        }

                        Loads.Remove(load);
                    }
                }
                if (Loads.Count != 0)
                    Direction = lastDirection;

                var thisType = this.GetType().ToString();

                //now load
                foreach (WaitingLoad waiting in waitingLoads.ToList())
                {
                    if (thisType.Contains(waiting.Type.ToString()))
                    {
                        if (thisType.Contains(waiting.Type.ToString()) && waiting.FloorNumber == Floor && (Direction == enStatus.Idle || Direction == waiting.Direction || !Loads.Any()))
                        {
                            waitingLoads.Remove(waiting);
                            Direction = enStatus.Idle;
                            //load
                            ElevatorLoad newLoad = new ElevatorLoad();
                            newLoad.DestinationFloor = waiting.DestinationFloor;
                            this.Loads.Add(newLoad);
                            while (Load < Capacity && waiting.Load > 0)
                            {
                                Load += 1;
                                waiting.Load -= 1;
                                newLoad.Load++;
                                //raise event
                                args.TimeReached = DateTime.Now;
                                OnMoveMentEvent(args);
                                //await Task.Delay(100);
                            }
                            Direction = waiting.Direction;

                            if (waiting.Load > 0)
                            {
                                waitingLoads.Add(waiting);
                            }

                            //raise event
                            args.TimeReached = DateTime.Now;
                            OnMoveMentEvent(args);
                            await Task.Delay(100);
                        }
                    }
                    //if (!Loads.Any())
                    //{
                    //    Direction = enStatus.Idle;
                    //}

                }

                if (Loads.Count == 0 && waitingLoads.Count == 0)
                    Direction = enStatus.Idle;
            }
            catch (Exception ex)
            {
                //log exception
                return;
            }
        }

        /// <summary>
        /// enables the elevator to opperate (this is set by default)
        /// </summary>
        public void Start()
        {
            Direction = enStatus.Idle;
        }

        /// <summary>
        /// stops the elevator from opperating
        /// </summary>
        public void Stop()
        {
            Direction = enStatus.OutOfOrder;
        }

        /////////////////////////
        ///EVENTS
        /////////////////////////

        public event ElevatorEventHandler ElevatorMoved;
        
        protected virtual void OnMoveMentEvent(ElevatorEventArgs e)
        {
            ElevatorMoved?.Invoke(this, e);
        }

    }
}
