using Spectre.Console;
using System.Globalization;

string choice = Menu.show();
ChoiceHandler choiceHandler = new();
choiceHandler.HandleChoice(choice);


internal class CodingSessionController
{
    internal void AddSession()
    {
        DateOnly date = getDate();
        TimeOnly startTime = getStartTime();
        TimeOnly endTime = getEndTime();

        CodingSession session = new(startTime, endTime, date);
        Repository repository = new();
        repository.InsertSession(session);
    }
    
    private TimeOnly getStartTime()
    {
        AnsiConsole.MarkupLine("Enter a START time in 24 hour format (HH:mm, e.g., 14:30): ");
        TimeOnly startTime = getTime();
        return startTime;
    }

    private TimeOnly getEndTime()
    {
        AnsiConsole.MarkupLine("Enter an END time in 24 hour format (HH:mm, e.g., 14:30): ");
        TimeOnly endTime = getTime();
        return endTime;
    }

    private TimeOnly getTime()
    {
        string input = AnsiConsole.Ask<string>("");
        string timeFormat = "HH:mm";

        while (true)
        {
            if (TimeOnly.TryParseExact(input, timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly targetTime))
            {
                AnsiConsole.MarkupLine($"Time entered: {targetTime}");
                return targetTime;
            }
            AnsiConsole.MarkupLine("Invalid time format. Please use the 24-hour HH:mm format.");
        }
    }
    private DateOnly getDate()
    {
       
        string input = AnsiConsole.Ask<string>("Enter a date (YYYY-MM-DD): ");
        string format = "yyyy-MM-dd";

        while (true)
        {
            if (DateOnly.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out DateOnly targetDate))
            {
                AnsiConsole.MarkupLine($"You entered: {targetDate}");
                return targetDate;
            }
            AnsiConsole.MarkupLine("Invalid date format. Please try again using YYYY-MM-DD.");
        }
    }
}

internal class Repository
{
    internal void InsertSession(CodingSession session)
    {
        
    }
}

internal class CodingSession
{
    private int _id;
    private TimeOnly _startTime;
    private TimeOnly _endtime;
    private DateOnly _date;


    internal CodingSession(TimeOnly startTime, TimeOnly endtime, DateOnly date)
    {
        _startTime = startTime;
        _endtime = endtime;
        _date = date;
    }
}
internal class Menu
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

internal class ChoiceHandler
{
    internal void HandleChoice(string choice)
    {
        switch (choice)
        {
            case "Add Coding Session":
                CodingSessionController sessionController = new();
                sessionController.AddSession();
                break;
        }
    }
}





