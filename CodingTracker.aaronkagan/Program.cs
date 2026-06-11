using Spectre.Console;

string choice = Menu.show();

switch (choice)
{
    case "Add Coding Session":
        
}


public class CodingSession
{
    private int _id;
    private TimeOnly _startTime;
    private TimeOnly _endtime;
    private DateOnly _date;


    public CodingSession(int id, TimeOnly startTime, TimeOnly endtime, DateOnly date)
    {
        _id = id;
        _startTime = startTime;
        _endtime = endtime;
        _date = date;
    }



}
public class Menu
{
    internal static string show()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please choose an option?")
                .AddChoices("View Coding Sessions", "Start Coding Session", "Add Coding Session", "Update Coding Session", "Delete Coding Session", "Exit Program"));
        return choice;
    }
}




