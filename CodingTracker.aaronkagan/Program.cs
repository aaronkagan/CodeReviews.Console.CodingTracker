using Spectre.Console;

string choice = Menu.show();

switch (choice)
{
    case "Add Coding Session":
        CodingSessionController sessionController = new();
        sessionController.AddSession();
        break;
}


internal class CodingSessionController
{
    private List<CodingSession> CodingSessions = [];

    internal void AddSession()
    {
        
    }
    private void getDate() {}
    private void getStartTime(){}
    private void getEndTime(){}
}

internal class Repository
{
    private void InsertSession()
    {
        
    }
}

public class CodingSession
{
    private int _id;
    private TimeOnly _startTime;
    private TimeOnly _endtime;
    private DateOnly _date;


    internal CodingSession(int id, TimeOnly startTime, TimeOnly endtime, DateOnly date)
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




