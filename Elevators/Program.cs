using Spectre.Console;
using Machine;
using Machine.Interfaces;
using ElevatorsConsole;
using Machine.Objects;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var floors = AnsiConsole.Prompt(new TextPrompt<int>( "How many floors does the building have?"));
var basments = AnsiConsole.Prompt(new TextPrompt<int>("How many basments levels does the building have?"));
var StandardCount = AnsiConsole.Prompt(new TextPrompt<int>("How many standard elevators in the building?"));
var GlassCount = AnsiConsole.Prompt(new TextPrompt<int>("How many glass elevators in the building?"));
var serviceCount = AnsiConsole.Prompt(new TextPrompt<int>("How many service elevators in the building?"));

ICentral Controller = new Central();

for (int i = 0; i < StandardCount; i++)
{
    Controller.AddAlivator(Machine.Objects.enElevatorType.Standard);
}
for (int i = 0; i < GlassCount; i++)
{
    Controller.AddAlivator(Machine.Objects.enElevatorType.Glass);
}
for (int i = 0; i < serviceCount; i++)
{
    Controller.AddAlivator(Machine.Objects.enElevatorType.Service);
}

AnsiConsole.Clear();

Building.PrintDetails(floors + Math.Abs(basments), StandardCount, GlassCount, serviceCount);

Runner runner = new();
runner.Run(Controller, floors, basments);

Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);



Controller.RequestElevator(enElevatorType.Glass, 1, 3, 5);

Console.ReadLine();

void myHandler(object sender, ConsoleCancelEventArgs args)
{

    args.Cancel = true;
    LoadRequestor.RequestElevator(Controller);
}