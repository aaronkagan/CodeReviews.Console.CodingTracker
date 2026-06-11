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

        Console.WriteLine(date);
        Console.WriteLine(startTime);
        Console.WriteLine(endTime);
    }

    private DateOnly getDate()
    {

        Console.Write("Enter a date (YYYY-MM-DD): ");
        string? input = Console.ReadLine();
        string format = "yyyy-MM-dd"; 
        if (DateOnly.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly targetDate))
        {
            Console.WriteLine($"You entered: {targetDate}");
        }
        else
        {
            Console.WriteLine("Invalid date format. Please try again using YYYY-MM-DD.");
        }
        return targetDate;

    }

    private TimeOnly getStartTime()
    {
        Console.Write("Enter a START time in 24 hour format (HH:mm, e.g., 14:30): ");
        string? input = Console.ReadLine();
        string timeFormat = "HH:mm";
        if (TimeOnly.TryParseExact(input, timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly startTime))
        {
            Console.WriteLine($"Time entered: {startTime}");
        }
        else
        {
            Console.WriteLine("Invalid time format. Please use the 24-hour HH:mm format.");
        }

        return startTime;
    }

    private TimeOnly getEndTime()
    {
        Console.Write("Enter an END time in 24 hour format (HH:mm, e.g., 14:30): ");
        string? input = Console.ReadLine();
        string timeFormat = "HH:mm";
        if (TimeOnly.TryParseExact(input, timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly endTime))
        {
            Console.WriteLine($"Time entered: {endTime}");
        }
        else
        {
            Console.WriteLine("Invalid time format. Please use the 24-hour HH:mm format.");
        }

        return endTime;
    }
}

internal class Repository
{
    private void InsertSession()
    {
        
    }
}

internal class CodingSession
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





