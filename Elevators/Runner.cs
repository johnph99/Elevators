using Machine.Interfaces;
using Machine.Objects;
using Spectre.Console;


namespace ElevatorsConsole
{
    public class Runner
    {
        public async void Run (ICentral controller, int floors, int basements)
        {
            controller.ElevatorMoved += Controller_ElevatorMoved;

            controller.Start(floors, basements);

            var table = new Table().Expand().BorderColor(Color.Grey);
            table.AddColumn("[yellow]Name[/]");
            table.AddColumn("[yellow]Capacity[/]");
            table.AddColumn("[yellow]Floor[/]");
            table.AddColumn("[yellow]Load[/]");
            table.AddColumn("[yellow]Status[/]");

            AnsiConsole.MarkupLine("Press [yellow]CTRL+C[/] to exit");

            await AnsiConsole.Live(table)
                .AutoClear(false)
                .Overflow(VerticalOverflow.Ellipsis)
                .Cropping(VerticalOverflowCropping.Bottom)
                .StartAsync(async ctx =>
                {
                    // Add some initial rows
                    foreach (Elevator elevator in  controller.Elevators)
                    {
                        AddElevatorRow(table, elevator);
                    }

                    // Continously update the table
                    while (true)
                    {
                        int x = 0; 
                        foreach (Elevator elevator in controller.Elevators)
                        {
                            table.Rows.RemoveAt(0);
                            AddElevatorRow(table, elevator);
                        }

                        //    // Refresh and wait for a while
                        ctx.Refresh();
                        await Task.Delay(400);
                        
                        CheckKeyPress(controller);
                    }
                });
        }

        private async void CheckKeyPress(ICentral controller)
        {
            //ConsoleKeyInfo key = new ConsoleKeyInfo();

            //if (AnsiConsole.Console.Input.IsKeyAvailable() )
            
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(true);
                var key = keyInfo.Key;
                if (key == ConsoleKey.A)
                {
                    LoadRequestor.RequestElevator(controller);
                }
            }
        }

        private void AddElevatorRow(Table table, IElevator elevator)
        {
            table.AddRow(elevator.Name, elevator.Capacity.ToString(), elevator.Floor.ToString(),
               elevator.Load == 0 ? $"[black]{elevator.Load}[/]" : $"[green]{elevator.Load}[/]"
               , elevator.Direction == enStatus.OutOfOrder ? $"[red]{elevator.Direction}[/]"
                : (elevator.Direction == enStatus.Idle ? $"[blue]{elevator.Direction}[/]" : $"[green]{elevator.Direction}[/]")
                );
        }

        void Controller_ElevatorMoved(Object sender, ElevatorEventArgs e)
        {
            //update the page
            //e.Elevator
            // var currentEleivator = Elevators.First(a => a.Name == e.Elevator.Name);
            // currentEleivator.Status = e.Elevator.Status;
            // currentEleivator.Floor = e.Elevator.Floor;
            // currentEleivator.Load = e.Elevator.Load;
            ;
            // StateHasChanged();

        }

       

    }
}
