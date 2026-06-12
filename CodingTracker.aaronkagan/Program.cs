using Microsoft.Data.Sqlite;
using Spectre.Console;
using System.Globalization;



Repository.InitializeDatabase();
string choice = Menu.show();
ChoiceHandler choiceHandler = new();
choiceHandler.HandleChoice(choice);


internal class CodingSessionController
{
    internal void AddSession()
    {
        DateOnly date = GetDate();
        TimeOnly startTime = GetStartTime();
        TimeOnly endTime = GetEndTime();
        CodingSession session = new(startTime, endTime, date);
        Repository.InsertSession(session);
    }
    
    private TimeOnly GetStartTime()
    {
        AnsiConsole.MarkupLine("Enter a START time in 24 hour format (HH:mm, e.g., 14:30): ");
        TimeOnly startTime = GetTime();
        return startTime;
    }

    private TimeOnly GetEndTime()
    {
        AnsiConsole.MarkupLine("Enter an END time in 24 hour format (HH:mm, e.g., 14:30): ");
        TimeOnly endTime = GetTime();
        return endTime;
    }

    private TimeOnly GetTime()
    {
        while (true)
        {
            string input = AnsiConsole.Ask<string>("");
            string timeFormat = "HH:mm";
            if (TimeOnly.TryParseExact(input, timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly targetTime))
            {
                AnsiConsole.MarkupLine($"Time entered: {targetTime}");
                return targetTime;
            }
            AnsiConsole.MarkupLine("Invalid time format. Please use the 24-hour HH:mm format.");
        }
    }
    private DateOnly GetDate()
    {
        while (true)
        {
            string input = AnsiConsole.Ask<string>("Enter a date (YYYY-MM-DD): ");
            string format = "yyyy-MM-dd";
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

internal static class Repository
{
    private static string _connectionString = "Data Source=coding-tracker.db";

    internal static void InitializeDatabase()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();

            var createTableSql = @"
            CREATE TABLE IF NOT EXISTS sessions (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                start_time TEXT NOT NULL,
                end_time TEXT NOT NULL,
                date TEXT NOT NULL
            );";

            using var command = new SqliteCommand(createTableSql, connection);
            command.ExecuteNonQuery();
        }
    }
    internal static void InsertSession(CodingSession session)
    {
        
    }

    internal static void SeedData()
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
                .AddChoices(
                    "View Coding Sessions",
                    "Start Coding Session",
                    "Add Coding Session", 
                    "Update Coding Session",
                    "Delete Coding Session",
                    "Exit Program"
                    ));
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

