namespace CodingTracker.aaronkagan;

public class CodingSession
{
    private int Id { get; set; }
    private DateTime StartTime { get; set; }
    private DateTime EndTime { get; set; }
    private TimeSpan Duration { get; set; }
}