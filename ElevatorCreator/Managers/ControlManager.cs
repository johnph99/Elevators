using Microsoft.AspNetCore.Mvc;
using SwitchBoard;

namespace ElevatorCreator.Managers
{
    [BindProperties]
    public class ControlManager
    {
        private Control controller = new Control();
        public bool test { get; set; }
        public async Task SetTest()
        {
            
            test = !test;
        }
        public bool GetTest() { return test; }
    }
}
