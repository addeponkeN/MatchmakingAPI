namespace Rovio.Matchmaking.Api.Services;

public class MatchmakingService : BackgroundService
{
    /// <summary>
    /// In milliseconds
    /// </summary>
    private int _updateFrequency;

    private MatchmakingManager _mm;

    public void Init(MatchmakingManager mm)
    {
        _mm = mm;
    }

    public void LoadSettings()
    {
        //  Temporary - should load from config/settings
        _updateFrequency = 50;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LoadSettings();

        while(!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_updateFrequency);
            // Log.Debug("matchmaking");
            _mm.Update();
        }
    }
}