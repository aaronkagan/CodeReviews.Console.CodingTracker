using Microsoft.Data.Sqlite;
using Spectre.Console;
using System.Globalization;
using Dapper;
using System.Data;

SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());
Repository.InitializeDatabase();
string choice = Menu.show();
ChoiceHandler choiceHandler = new();
choiceHandler.HandleChoice(choice);


internal class CodingSessionController
{
    internal void ViewSessions()
    {
        List<CodingSession> codingSessions = Repository.ReadSessions();
        
        var table = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey)
            .Title("[yellow bold]Coding Sessions[/]");
  
        table.AddColumn("ID");
        table.AddColumn("Date", col => col.Centered());
        table.AddColumn("Start Time", col => col.Centered());
        table.AddColumn("End Time", col => col.RightAligned());
        
        foreach (var codingSession in codingSessions)
        {
            table.AddRow(codingSession.Id.ToString(), codingSession.Date.ToString(), codingSession.StartTime.ToString(), codingSession.EndTime.ToString());
        }
        
        AnsiConsole.Write(table);
    }
    
    internal void AddSession()
    {
        DateOnly date = GetDate();
        TimeOnly startTime = GetStartTime();
        TimeOnly endTime;
        
        while (true)
        {
            endTime = GetEndTime();
            if (endTime <= startTime)
            {
                AnsiConsole.MarkupLine("You cannot extend your coding session to the next day");
                AnsiConsole.MarkupLine("Please end your session at 23:59 and create a new session for the next day starting at 00:00.");
            }
            else
            {
                break;
            }
        }
       
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
    private static readonly string _connectionString = "Data Source=coding-tracker.db";

    internal static void InitializeDatabase()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            var createTableSql = @"
                    CREATE TABLE IF NOT EXISTS sessions (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    start_time TEXT NOT NULL,
                    end_time TEXT NOT NULL,
                    date TEXT NOT NULL
        );";

            connection.Execute(createTableSql);
        }

    }
    internal static void InsertSession(CodingSession session)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            var sql = "INSERT INTO sessions (date, start_time, end_time) VALUES (@Date, @StartTime, @EndTime)";
            {
                var anonymousSession = new { Date = session.Date, StartTime = session.StartTime, EndTime = session.EndTime };
                var rowsAffected = connection.Execute(sql, anonymousSession);
                Console.WriteLine($"{rowsAffected} row(s) inserted.");
            }
        }
    }

    internal static List<CodingSession> ReadSessions()
    {
        string sql = "SELECT * FROM sessions";

        using (var connection = new SqliteConnection(_connectionString))
        {            
            var codingSessions = connection.Query<CodingSession>(sql).ToList();

            return codingSessions;
        }
    }

    // internal static void SeedData()
    // {
    //     
    // }
}

internal class CodingSession
{
    public int Id { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public DateOnly Date { get; init; }
    // Empty constructor for use by Dapper Reflection.
    internal CodingSession() { } 
    internal CodingSession(TimeOnly startTime, TimeOnly endtime, DateOnly date)
    {
        StartTime = startTime;
        EndTime = endtime;
        Date = date;
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
        CodingSessionController sessionController = new();
        switch (choice)
        {
            case "Add Coding Session":
                sessionController.AddSession();
                break;
            case "View Coding Sessions":
                sessionController.ViewSessions();
                break;
        }
    }
}


public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.Value = value.ToString("yyyy-MM-dd");
    }

    public override DateOnly Parse(object value)
    {
        return DateOnly.Parse(value.ToString()!);
    }
}
public class TimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
{
    public override void SetValue(IDbDataParameter parameter, TimeOnly value)
    {
        parameter.Value = value.ToString("HH:mm:ss");
    }

    public override TimeOnly Parse(object value)
    {
        return TimeOnly.Parse(value.ToString()!);
    }
}
