using Machine.Interfaces;
using Machine.Objects;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorsConsole
{
    public static class LoadRequestor
    {
        public static string RequestElevator(ICentral controller)
        {
            string retval = "";

            AnsiConsole.AlternateScreen(() =>
            {

                // Now we're in another terminal screen buffer
                AnsiConsole.Write(new Rule("[red]Request Elevator[/]"));
                WriteDivider("Elevator Type");
                var typ = AskType();
                WriteDivider("From Floor");
                int fromFloor = GetInput("load", controller.Elevators[0].BottomFloor, controller.Elevators[0].TopFloor);
                WriteDivider("To Floor");
                int toFloor = GetInput("offload", controller.Elevators[0].BottomFloor, controller.Elevators[0].TopFloor);
                WriteDivider("Load");
                int load = AskLoad();
                
                controller.RequestElevator(typ, fromFloor, toFloor, load);

                retval= $"Type: {typ.ToString()}, From Floor: {fromFloor}, To Floor: {toFloor}, Load: {load}";
            });

            return retval;
        }

        private static int AskLoad()
        {
            int load = AnsiConsole.Prompt(new TextPrompt<int>("Enter the number of passagers/load")
                    .PromptStyle("green")
                    .ValidationErrorMessage("[red]That's not a valid load number[/]")
                    .Validate(ld =>
                    {
                        return ld switch
                        {
                            <= 0 => ValidationResult.Error("[red]Load must be at least 0 [/]"),
                            >= 50 => ValidationResult.Error("[red]Max load is 50[/]"),
                            _ => ValidationResult.Success(),
                        };
                    })
                    );
            return load;

        }

        private static int GetInput(string stop, int bottomFloor, int topFloor)
        {
            if (stop == "load")
                stop = "What is the loading floor number?";
            else
                stop = "What is the offloading floor number?";

            int floor = AnsiConsole.Prompt(new TextPrompt<int>(stop)
                     .PromptStyle("green")
                     .ValidationErrorMessage("[red]That's not a valid floor number[/]")
                    );
            if (floor<bottomFloor) floor = bottomFloor;
            if (floor>topFloor)  floor=topFloor ;

            return floor;

        }

        public static void WriteDivider(string text)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule($"[yellow]{text}[/]").RuleStyle("grey").LeftJustified());
        }

        private static enElevatorType AskType()
        {
            var typeString = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .PageSize(3)
                                .Title("What elevator do you want to call?")
                                .AddChoices(new[]{"Standard", "Glass", "Service"}));
            switch (typeString)
            {
                case "Glass":
                    return enElevatorType.Glass;
                    break;
                case "Service":
                    return enElevatorType.Service;
                    break;
                default:
                    return enElevatorType.Standard;
                    break;
            }
        }

    }
}
