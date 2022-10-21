namespace Rovio.Utility;

internal static class EnumStringCache<T> where T : Enum
{
    private static Dictionary<T, string> _cache;

    internal static string Get(T @enum)
    {
        _cache ??= new();
        if(!_cache.TryGetValue(@enum, out var value))
            _cache.Add(@enum, value = @enum.ToString());
        return value;
    }
}

public static class EnumExtensions
{
    public static string ToStringEnum<T>(this T @enum) where T : Enum
    {
        return EnumStringCache<T>.Get(@enum); 
    }
}
