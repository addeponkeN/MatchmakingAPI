namespace Rovio.Matchmaking.Api.Settings;

public class MatchmakingApiSettings
{
    public static MatchmakingApiSettings Default = new()
    {
        ListenUrls = new() {"https://*:5000", "http://*:5001"}
    };

    public List<string> ListenUrls { get; init; }
}