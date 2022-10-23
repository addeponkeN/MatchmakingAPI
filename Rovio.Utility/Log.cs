namespace Rovio.Utility;

public static class Log
{
    public delegate string DateTimeFormat();

    /// <summary>
    /// Will print a timestamp based on the <see cref="Format"/> delegate
    /// </summary>
    public static bool ShowTimestamp = true;

    public static DateTimeFormat Format = () => _centralEuTime.ToShortTimeString();

    private static ConsoleExtended.PrintLine _printLine = Console.WriteLine;
    private static ConsoleExtended.PrintLine _print = Console.Write;

    private static DateTime _centralEuTime = DateTime.UtcNow.Add(TimeSpan.FromHours(2));

    public static void SetPrintFunction(ConsoleExtended.PrintLine print)
    {
        _print = print;
    }

    public static void SetPrintLineFunction(ConsoleExtended.PrintLine print)
    {
        _printLine = print;
    }

    /// <summary>
    /// Debug logging
    /// </summary>
    /// <param name="msg">Console message</param>
    public static void Debug(string msg)
    {
        Console.ResetColor();
        
        if(ShowTimestamp)
        {
            _print($"[{Format()}]: ");
        }

        _printLine(msg);
    }

    public static void Debug()
    {
        _printLine("");
    }

    /// <summary>
    /// Warning text, Yellow
    /// </summary>
    /// <param name="msg">Console message</param>
    public static void Warning(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;

        if(ShowTimestamp)
        {
            _print($"[{Format()}]: ");
        }

        _printLine(msg);

        Console.ResetColor();
    }
}