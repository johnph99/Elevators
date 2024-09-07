using SwitchBoard.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SwitchBoard.Control;

namespace SwitchBoard
{
    public interface IControl
    {
        Elevator AddAlivator(enElivatorType type);
        void Start(int noOfFloors = 5, int noOfBAsements = 0);
        void Stop();

        delegate void ElevatorEventHandler(Object sender, ElevatorEventArgs e);

        event ElevatorEventHandler ElevatorMoved;

        void RaiseMoveMentEvent(ElevatorEventArgs e);
        void CallElevators(enElivatorType type, int currentFloor, int destinationFloor, int load);


    }
}
