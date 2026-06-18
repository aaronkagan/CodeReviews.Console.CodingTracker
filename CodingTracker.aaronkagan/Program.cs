using Microsoft.Data.Sqlite;
using Spectre.Console;
using System.Globalization;
using Dapper;
using System.Data;
using Rule = Spectre.Console.Rule;

SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());
SqlMapper.AddTypeHandler(new TimeSpanTypeHandler());

CodingSessionController sessionController = new();
sessionController.Start();

internal static  class Helpers
{

    internal static void AddProgress(string message)
    {
        AnsiConsole.Progress()
            .Start(ctx =>
            {
                var task = ctx.AddTask(message, maxValue: 100);
  
                while (!ctx.IsFinished)
                {
                    task.Increment(1);
                    Thread.Sleep(17);
                }
            });
    }
    internal static DateOnly GetDate()
    {
        while (true)
        {
            string input = AnsiConsole.Ask<string>("\nEnter a date (YYYY-MM-DD) or type T then ENTER to use today's date:");
            
            string format = "yyyy-MM-dd";
            if (input.ToLower() == "t")
            {
                input = DateOnly.FromDateTime(DateTime.Now).ToString(format);
            }
            if (DateOnly.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out DateOnly targetDate))
            {
                return targetDate;
            }
            AnsiConsole.MarkupLine("\nInvalid date format. Please try again using YYYY-MM-DD.");
        }
    }
    internal static int GetId()
    {
        while (true)
        {
            var userInput = AnsiConsole.Ask<string>("\nPlease Enter the ID of the row.");
            if (!Int32.TryParse(userInput, out int id))
            {
                AnsiConsole.MarkupLine("\nInvalid input. Please enter number.");
            }  
            if (Repository.CheckIfId(id))
            {   
                return id;
            }
            AnsiConsole.MarkupLine("\nError. No row with that Id.");
        }
    }
    internal static TimeOnly GetStartTime()
    {
        AnsiConsole.MarkupLine("\nEnter a START time in 24 hour format (HH:mm, e.g., 14:30): ");
        TimeOnly startTime = GetTime();
        return startTime;
    }
    internal static TimeOnly GetEndTime()
    {
        AnsiConsole.MarkupLine("\nEnter an END time in 24 hour format (HH:mm, e.g., 14:30): ");
        TimeOnly endTime = GetTime();
        return endTime;
    }
    private static TimeOnly GetTime()
    {
        while (true)
        {
            string input = AnsiConsole.Ask<string>("");
            string timeFormat = "HH:mm";
            if (TimeOnly.TryParseExact(input, timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly targetTime))
            {
                return targetTime;
            }
            AnsiConsole.MarkupLine("\nInvalid time format. Please use the 24-hour HH:mm format.");
        }
    }
    internal static TimeSpan CalculateDuration(TimeOnly startTime, TimeOnly endTIme)
    {
        TimeSpan duration = TimeSpan.Parse((endTIme - startTime).ToString(@"hh\:mm\:ss"));
        return duration;
    }

    internal static void ReturnToMainMenu()
    {
        AnsiConsole.MarkupLine("\nPress any key to return to the main menu");
        Console.ReadKey();
        Console.Clear();
        
        string choice = Menu.Show();
        ChoiceHandler.HandleChoice(choice);
    }
}
internal class CodingSessionController
{
    internal void Start()
    {
        Repository.InitializeDatabase();
        Console.Clear();
        Console.WriteLine("\n\n");
        
        var blockFont = FigletFont.Load("block.flf");

        var banner = new FigletText(blockFont,"CODING TRACKER")
        {
            Color = Color.Teal,
            Justification = Justify.Center
        };
  
        AnsiConsole.Write(banner);
        
        string choice = Menu.Show();
        ChoiceHandler.HandleChoice(choice);
    }

    internal void ViewSessions(bool returnToMenu=false)
    {
        Console.Clear();
        List<CodingSession> codingSessions = Repository.ReadSessions();
        
        var table = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey)
            .Title("[yellow bold]Coding Sessions[/]");
  
        table.AddColumn("ID");
        table.AddColumn("Date", col => col.Centered());
        table.AddColumn("Start Time", col => col.Centered());
        table.AddColumn("End Time", col => col.Centered());
        table.AddColumn("Duration", col => col.RightAligned());
        
        foreach (var codingSession in codingSessions)
        {
            table.AddRow(codingSession.Id.ToString(), codingSession.Date.ToString(), codingSession.StartTime.ToString(), codingSession.EndTime.ToString(), codingSession.Duration.ToString());
        }
        
        AnsiConsole.Write(table);
        
        
        if (returnToMenu)
        {
            Helpers.ReturnToMainMenu();
        }
    }
    internal void StartSession()
    {
        Console.Clear();
        DateOnly date = DateOnly.FromDateTime(DateTime.Today);
        TimeOnly startTime = TimeOnly.FromDateTime(DateTime.Now);
        AnsiConsole.MarkupLine($"\nSession Started at {startTime}.");

        
        while (!Console.KeyAvailable)
        {
            Console.Clear();
            AnsiConsole.MarkupLine($"\nSession Started at {startTime}. Press any key to end the session\n");
            var figlet = new FigletText((TimeOnly.FromDateTime(DateTime.Now) - startTime).ToString(@"hh\:mm\:ss"))
            {
                Color = Color.Green,
                Justification = Justify.Center,
            };
            var panel = new Panel(figlet)
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Green),
                Padding = new Padding(1, 1, 1, 1)
            };
            AnsiConsole.Write(panel);
            Thread.Sleep(1000);
            
          
        }
        
        TimeOnly endTime = TimeOnly.FromDateTime(DateTime.Now);
        Console.Clear();
        AnsiConsole.MarkupLine($"\nSession ended at {endTime}.");
        CodingSession session = new(startTime, endTime, date);
        Helpers.AddProgress("\nSaving session...");
        Repository.InsertSession(session);

        Console.ReadKey();
        Helpers.ReturnToMainMenu();
    }
    internal void AddSession()
    {
        DateOnly date = Helpers.GetDate();
        TimeOnly startTime = Helpers.GetStartTime();
        TimeOnly endTime;
        
        while (true)
        {
            endTime = Helpers.GetEndTime();
            if (endTime <= startTime)
            {
                AnsiConsole.MarkupLine("\nYou cannot extend your coding session to the next day");
                AnsiConsole.MarkupLine("\nPlease end your session at 23:59 and create a new session for the next day starting at 00:00.");
            }
            else
            {
                break;
            }
        }
       
        CodingSession session = new(startTime, endTime, date);
        
        Console.Clear();
        Helpers.AddProgress("\nAdding session...");
        
        Repository.InsertSession(session);
        
        Helpers.ReturnToMainMenu();

    }
 
    internal void DeleteSession()
    {
        CodingSessionController sessionController = new();
        sessionController.ViewSessions(false);
        
        int id = Helpers.GetId();
        
        Repository.DeleteSession(id);
        Console.Clear();
        Helpers.AddProgress("\nDeleting session...");
        AnsiConsole.MarkupLine($"\nDelete Successful!");
        
        
        Helpers.ReturnToMainMenu();
    }
    internal void UpdateSession()
    {
        CodingSessionController sessionController = new();
        sessionController.ViewSessions();
        while (true)
        {
            var id = Helpers.GetId();
            var date = Helpers.GetDate();
            var startTime = Helpers.GetStartTime();
            var endTime = Helpers.GetEndTime();
            
            bool updated = Repository.UpdateSession(id, date, startTime, endTime);
            if (updated)
            { 
                Console.Clear();
                Helpers.AddProgress("\nUpdating session...");
                AnsiConsole.MarkupLine($"\nUpdate Successful!");
                break;
            }
        }
        
        Helpers.ReturnToMainMenu();
    }
    internal void ExitProgram()
    {
        Console.Clear();
        var banner = new FigletText("\nExiting Program. Goodbye!")
        {
            Color = Color.Red,
            Justification = Justify.Center
        };
        
        AnsiConsole.Write(banner);
    }
}

internal static class Repository
{
    private static readonly string ConnectionString = "Data Source=coding-tracker.db";
    internal static void InitializeDatabase()
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            var createTableSql = @"
                    CREATE TABLE IF NOT EXISTS sessions (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    start_time TEXT NOT NULL,
                    end_time TEXT NOT NULL,
                    duration TEXT NOT NULL,
                    date TEXT NOT NULL
        );";
            connection.Execute(createTableSql);
        }
    }
    internal static void InsertSession(CodingSession session, string mode="")
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            var sql = "INSERT INTO sessions (date, start_time, end_time, duration) VALUES (@Date, @StartTime, @EndTime, @Duration)";
            {
                var anonymousSession = new { session.Date, session.StartTime, session.EndTime, session.Duration};
                var rowsAffected = connection.Execute(sql, anonymousSession);

                if (mode != "quietly")
                {
                    Console.WriteLine($"\n{rowsAffected} row(s) inserted.");
                }
            }
        }
    }
    

    internal static List<CodingSession> ReadSessions()
    {
        string sql = @"SELECT
                     id AS ID,
                     date AS Date,
                     start_time AS StartTime,
                     end_time AS EndTime,
                     duration As Duration
                     FROM sessions";

        using (var connection = new SqliteConnection(ConnectionString))
        {
            var codingSessions = connection.Query<CodingSession>(sql).ToList();
            return codingSessions;
        }
    }
    
    internal static bool DeleteSession(int id)
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            var sql = $"DELETE FROM sessions WHERE id = @Id";		
            var rowsAffected = connection.Execute(sql, new {Id = id});
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }
    }

    internal static bool CheckIfId(int id)
    {
        using (var connection = new SqliteConnection(ConnectionString))
        {
            var sql = "SELECT EXISTS(SELECT 1 FROM sessions WHERE id = @Id)";
            bool exists = connection.ExecuteScalar<bool>(sql, new { Id = id });
            return exists;
        }
    }
    
    internal static bool UpdateSession(int id, DateOnly date, TimeOnly startTime, TimeOnly endTime)
    {
        TimeSpan duration = Helpers.CalculateDuration(startTime, endTime);
        using (var connection = new SqliteConnection(ConnectionString))
        {
            var rowsAffected = connection.Execute(
                "UPDATE sessions SET date = @date, start_time = @startTime, end_time = @endTime, duration = @duration  WHERE id = @id", 
                new { id, date, startTime, endTime, duration}
            ); 
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }
    }

    internal static void SeedData()
    {
        List<CodingSession> entries =
        [
            new (new TimeOnly(09, 00), new TimeOnly(10, 30), new DateOnly(2026, 01, 02)),
            new (new TimeOnly(11, 00), new TimeOnly(12, 15), new DateOnly(2026, 01, 03)),
            new (new TimeOnly(13, 00), new TimeOnly(14, 45), new DateOnly(2026, 01, 04)),
            new (new TimeOnly(18, 00), new TimeOnly(19, 30), new DateOnly(2026, 01, 05)),
            new (new TimeOnly(20, 15), new TimeOnly(21, 00), new DateOnly(2026, 01, 06)),

            new (new TimeOnly(08, 30), new TimeOnly(09, 45), new DateOnly(2026, 01, 07)),
            new (new TimeOnly(10, 00), new TimeOnly(11, 20), new DateOnly(2026, 01, 08)),
            new (new TimeOnly(14, 00), new TimeOnly(15, 30), new DateOnly(2026, 01, 09)),
            new (new TimeOnly(16, 00), new TimeOnly(17, 10), new DateOnly(2026, 01, 10)),
            new (new TimeOnly(19, 00), new TimeOnly(20, 30), new DateOnly(2026, 01, 11)),

            new (new TimeOnly(09, 15), new TimeOnly(10, 00), new DateOnly(2026, 01, 12)),
            new (new TimeOnly(10, 30), new TimeOnly(12, 00), new DateOnly(2026, 01, 13)),
            new (new TimeOnly(13, 30), new TimeOnly(15, 00), new DateOnly(2026, 01, 14)),
            new (new TimeOnly(15, 15), new TimeOnly(16, 45), new DateOnly(2026, 01, 15)),
            new (new TimeOnly(18, 30), new TimeOnly(20, 00), new DateOnly(2026, 01, 16)),

            new (new TimeOnly(09, 00), new TimeOnly(10, 10), new DateOnly(2026, 01, 17)),
            new (new TimeOnly(10, 20), new TimeOnly(11, 40), new DateOnly(2026, 01, 18)),
            new (new TimeOnly(12, 00), new TimeOnly(13, 30), new DateOnly(2026, 01, 19)),
            new (new TimeOnly(14, 10), new TimeOnly(15, 40), new DateOnly(2026, 01, 20)),
            new (new TimeOnly(17, 00), new TimeOnly(18, 20), new DateOnly(2026, 01, 21)),
        ];
        
        foreach (var codingSession in entries)
        {
            InsertSession(codingSession, "quietly");
        }
        Console.Clear();
        Helpers.AddProgress("\nSeeding data...");
        AnsiConsole.MarkupLine("\nData Seeded.");
        
        Helpers.ReturnToMainMenu();
        
    }
}

internal class CodingSession
{
    public int Id { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public DateOnly Date { get; init; }
    public TimeSpan Duration { get; init; }
    // Empty constructor for use by Dapper Reflection.
    internal CodingSession() { } 
    internal CodingSession(TimeOnly startTime, TimeOnly endtime, DateOnly date)
    {
        StartTime = startTime;
        EndTime = endtime;
        Date = date;
        Duration = Helpers.CalculateDuration(startTime, endtime);
    }
}
internal static class Menu
{
    internal static string Show()
    {
        
        var prompt = new SelectionPrompt<string>()
            .Title("\nChoose an option:")
            .HighlightStyle(new Style(foreground: Color.Pink1))
            .AddChoices(
                "View Coding Sessions",
                "Start Coding Session",
                "Add Coding Session",
                "Update Coding Session",
                "Delete Coding Session",
                "Seed Data",
                "Exit Program"
            );
        
        var choice = AnsiConsole.Prompt(prompt);
        return choice;
    }
}
internal static class ChoiceHandler
{
    internal static void HandleChoice(string choice)
    {
        CodingSessionController sessionController = new();
        switch (choice)
        {
            case "View Coding Sessions":
                sessionController.ViewSessions(true);
                break;
            case "Start Coding Session":
                sessionController.StartSession();
                break;
            case "Add Coding Session":
                sessionController.AddSession();
                break;
            case "Update Coding Session":
                sessionController.UpdateSession();
                break;
            case "Delete Coding Session":
                sessionController.DeleteSession();
                break;
            case "Seed Data":
                Repository.SeedData();
                break;
            case "Exit Program":
                sessionController.ExitProgram();
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
public class TimeSpanTypeHandler : SqlMapper.TypeHandler<TimeSpan>
{
    public override void SetValue(IDbDataParameter parameter, TimeSpan value)
    {
        parameter.Value = value.ToString("HH:mm:ss");
    }

    public override TimeSpan Parse(object value)
    {
        return TimeSpan.Parse(value.ToString()!);
    }
}
