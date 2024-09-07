using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine.Objects
{
    public class WaitingLoad
    {
        public int FloorNumber{get;set;}
        public int Load{get;set;}
        public int DestinationFloor{get;set;}
        public enElivatorType Type { get; set; }


        public enStatus Direction
        {
            get 
            {
                enStatus retVal = enStatus.Idle;
                if (FloorNumber > DestinationFloor)
                {
                    retVal = enStatus.MovingDown;
                }
                else if (DestinationFloor > FloorNumber)
                {
                    retVal = enStatus.MovingUp;
                }
                return retVal;
            }
        }
    }
}
