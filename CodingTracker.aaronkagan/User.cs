namespace CodingTracker.aaronkagan;

public class User
{
    internal string Name { get; private set; }
    public User(string name)
    {
        Name = name;
    }
}