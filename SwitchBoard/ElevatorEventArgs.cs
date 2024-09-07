using SwitchBoard.Objects;

namespace SwitchBoard
{
    public class ElevatorEventArgs : EventArgs
    {
        public Elevator Elevator { get; set; }
        public DateTime TimeReached { get; set; }
    }

  
}
