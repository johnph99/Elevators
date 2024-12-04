
namespace Machine.Objects
{
    public class ElevatorEventArgs : EventArgs
    {
        public Elevator Elevator { get; set; }
        public DateTime TimeReached { get; set; }
    }

    public delegate void ElevatorEventHandler(Object sender, ElevatorEventArgs e);

}
