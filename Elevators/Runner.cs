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

            //_ = Task.Run(() => ListenForKeyboardInput(controller));

            var table = new Table().Expand().BorderColor(Color.Grey);
            table.AddColumn("[yellow]Name[/]");
            table.AddColumn("[yellow]Capacity[/]");
            table.AddColumn("[yellow]Floor[/]");
            table.AddColumn("[yellow]Load[/]");
            table.AddColumn("[yellow]Status[/]");

            bool addmore = true;
            while (addmore)
            {
                string callDesc = LoadRequestor.RequestElevator(controller);
                AnsiConsole.WriteLine(callDesc);

                addmore = AnsiConsole.Confirm("Do you want to add another elevator call?");
            }
            
            AnsiConsole.MarkupLine("Press [yellow]CTRL+C[/] to exit");

            _ = AnsiConsole.Live(table)
                .AutoClear(false)
                .Overflow(VerticalOverflow.Ellipsis)
                .Cropping(VerticalOverflowCropping.Bottom)
                .StartAsync(async ctx =>
                {
                    // Add some initial rows
                    foreach (Elevator elevator in controller.Elevators)
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
                    }
                });
        }

        //private static async void ListenForKeyboardInput(ICentral controller)
        //{
        //    while (true)
        //    {
        //        // Read a line from the console  
        //        var key = Console.ReadKey().Key;
        //        if (key == ConsoleKey.A)
        //        {
        //            Paused = true;
        //            await Task.Delay(500);
        //            LoadRequestor.RequestElevator(controller);
        //            Paused = false;
        //        }
        //    }
        //}

       
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
        }

       

    }
}
