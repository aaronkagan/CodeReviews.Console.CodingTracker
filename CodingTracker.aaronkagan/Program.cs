using Spectre.Console;

Menu.show();


public class CodingSession
{
    private int Id { get; set; }
    private TimeOnly StartTime { get; set; }
    private TimeOnly EndTime { get; set; }
    private DateOnly Date { get; set; }
    
}
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




