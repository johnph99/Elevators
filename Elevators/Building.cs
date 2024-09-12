using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorsConsole
{
    internal static class Building
    {
        public static void PrintDetails(int floors, int sCount, int gCount, int vCount)
        {
            var tble = new Table()
             .Border(TableBorder.Rounded)
             .BorderColor(Color.Green)
             .AddColumn(new TableColumn("[u]Floors[/]"))
             .AddColumn(new TableColumn("[u]Standard Elevators[/]"))
             .AddColumn(new TableColumn("[u]Glass Elevators[/]"))
             .AddColumn(new TableColumn("[u]Service Elevators[/]"))
             .AddRow($"[blue]{floors}[/]", $"[yellow]{sCount}[/]", $"[yellow]{gCount}[/]", $"[yellow]{vCount}[/]");

            AnsiConsole.Write(new Panel(tble.Centered()).Expand()
                .AsciiBorder()
                .Header("[green]Biulding[/]")
                .HeaderAlignment(Justify.Center));
            //return tble;
        }
    }
}
