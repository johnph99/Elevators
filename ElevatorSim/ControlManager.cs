namespace ElevatorSim
{
    public class ControlManager
    {
        public bool test { get; set; }
        public async Task SetTest()
        {
            test = !test;
        }
        public bool GetTest() { return test; }
    }
}
