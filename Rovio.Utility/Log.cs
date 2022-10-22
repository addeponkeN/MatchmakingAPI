namespace Rovio.Utility;

public static class Log
{
    public delegate string DateTimeFormat();

    private static DateTime _centralEuTime = DateTime.UtcNow.Add(TimeSpan.FromHours(2));

    public static DateTimeFormat Format = () => _centralEuTime.ToShortTimeString();

    /// <summary>
    /// Will show a timestamp based on the Format delegate
    /// </summary>
    public static bool ShowTimestamp = true;

    /// <summary>
    /// Debug logging
    /// </summary>
    /// <param name="msg">Console message</param>
    public static void Debug(string msg)
    {
        if(ShowTimestamp)
            Console.Write($"[{Format()}]: ");
        Console.WriteLine(msg);
    }
    
}