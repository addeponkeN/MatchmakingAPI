
namespace Rovio.Matchmaking.Api.Services;

public class MatchmakingService : BackgroundService
{
    /// <summary>
    /// In milliseconds
    /// </summary>
    private int _updateFrequency;
    
    private MatchmakingManager _manager;

    public void Init(MatchmakingManager mm)
    {
        _manager = mm;
    }
    
    //  todo : load from config
    public void LoadSettings()
    {
        _updateFrequency = 50;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LoadSettings();
        
        while(!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_updateFrequency);
            // Log.Debug("matchmaking");
            _manager.Update();
        }
    }
}