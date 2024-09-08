using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Machine.Objects
{
    public abstract class Elevator
    {
        public string Name { get; set; }
        /// <summary>
        /// Indicates the max number of persons or items that can be loaded into the elevator
        /// </summary>
        public abstract int Capacity { get; }
        /// <summary>
        /// Get or set the current number of occupants
        /// </summary>
        public int Load { get; set; }
        /// <summary>
        /// Indicates the current floor, if the number is between 2 integer values, the elevator is between floors
        /// </summary>
        public double Floor { get; set; } = 0;
        public enStatus Direction { get; set; } = enStatus.Idle;
        /// <summary>
        /// Contains a list of floors the elevator should stop at
        /// </summary>
        public List<ElevatorLoad> Loads { get; } = new();
        public int nextStop { get; set; }

        public async void Move()
        {
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
            await Task.Delay(100);

            ////check if we reached a floor number
            //if (Direction != enStatus.Idle && Floor == Math.Round(Floor, 0))
            //{
            //    //check if we must unload and load
            //    //or maybe do this from another call
            //}
        }


        //we need to get the waiting loads at the floor number for loading
        //for offloading, the elevator should have the list of items to offload

        public async void LoadUnload(List<WaitingLoad> waitingLoads)
        {
            //check if we need to offload anything on the current floor
            foreach (var load in Loads) 
            { 
                if (load.DestinationFloor==Floor)
                {
                    //offload
                    for (var i = 0; i < load.Load; i++)
                    {
                        Load -= 1;
                        //raise event
                        ElevatorEventArgs args = new();
                        args.Elevator = this;
                        args.TimeReached = DateTime.Now;
                        OnMoveMentEvent(args);
                    }

                    Loads.Remove(load);
                }
            }
            
            //now load
            foreach (WaitingLoad waiting in waitingLoads)
            {
                if (Direction == enStatus.Idle || Direction == waiting.Direction)
                {
                    if (waiting.FloorNumber == Floor)
                    {
                        waitingLoads.Remove(waiting);   

                        //load
                        while (Load < Capacity && waiting.Load > 0)
                        {
                            Load += 1;
                            waiting.Load -= 1;
                            //raise event
                        }

                        if (waiting.Load > 0)
                        {
                            waitingLoads.Add(waiting);
                        }
                    }
                }
            }
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
