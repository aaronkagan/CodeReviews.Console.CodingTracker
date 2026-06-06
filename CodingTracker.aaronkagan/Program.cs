using CodingTracker.aaronkagan;
using Spectre.Console;

AnsiConsole.MarkupLine("[green]Welcome to the Coding Tracker[/]");
var name = AnsiConsole.Ask<string>("What is your name?: ");
var user = new User(name);
AnsiConsole.MarkupLine($"Welcome, [yellow]{user.Name}[/]");

Menu.show();



