namespace CodingTracker.aaronkagan;
using Spectre.Console;


public class Menu
{
    internal static void show()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please choose an option?")
                .AddChoices("1", "2", "3", "Exit Program"));
    }
}