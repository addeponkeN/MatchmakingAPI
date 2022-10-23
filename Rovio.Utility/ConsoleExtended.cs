namespace Rovio.Utility;

public static class ConsoleExtended
{
    public delegate void PrintLine(string msg);
    private static PrintLine _print = Console.WriteLine;

    public static void SetPrintFunction(PrintLine print)
        => _print = print;
    
    public static T RequestChoice<T>(string title, params T[] choices)
    {
        string key;
        int choice;
        do
        {
            Console.Clear();
            _print(title);
            for(int i = 0; i < choices.Length; i++)
                _print($"{i + 1}) {choices[i].ToString()}");

            key = Console.ReadKey().KeyChar.ToString();
        } while(!int.TryParse(key, out choice) || choice <= 0 || choice  > choices.Length);

        return choices[choice - 1];
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="choices"></param>
    /// <returns>Returns the index of the selected item</returns>
    public static int RequestChoice(string title, params string[] choices)
    {
        string key;
        int choice;
        do
        {
            Console.Clear();
            _print(title);
            for(int i = 0; i < choices.Length; i++)
                _print($"{i + 1}) {choices[i]}");

            key = Console.ReadKey().KeyChar.ToString();
        } while(!int.TryParse(key, out choice) || choice <= 0 || choice  > choices.Length);

        return choice - 1;
    }
    
    public static int RequestNumber(string tx)
    {
        int ret;
        do
        {
            _print(tx);
        } while(!int.TryParse(Console.ReadLine(), out ret));

        return ret;
    }
    
    public static string RequestString(string title)
    {
        _print(title);
        return Console.ReadLine();
    }

}