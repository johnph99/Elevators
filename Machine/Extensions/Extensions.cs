using Machine.Objects;

namespace Machine.Extensions
{
    public static class Extensions
    {
        public static double Distance(this Elevator elevator, int FromFloor)
        {
            return Math.Abs(elevator.Floor - FromFloor);
        }

        public static double Distance(this WaitingLoad load, int ToFloor)
        {
            return Math.Abs(load.FloorNumber - ToFloor);
        }
    }
}
