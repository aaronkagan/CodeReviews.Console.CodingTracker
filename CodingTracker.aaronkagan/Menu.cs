namespace CodingTracker.aaronkagan;
using Spectre.Console;


public class Menu
{
    internal static void show()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please choose an option?")
                .AddChoices("View Coding Sessions", "Start Coding Session", "Add Coding Session", "Update Coding Session", "Delete Coding Session", "Exit Program"));
    }
}