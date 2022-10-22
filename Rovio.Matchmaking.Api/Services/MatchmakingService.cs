
namespace Rovio.Matchmaking.Api.Services;

public class MatchmakingService : BackgroundService
{
    /// <summary>
    /// In milliseconds
    /// </summary>
    private int _updateFrequency;
    
    private Matchmaker _mm;

    public void Init(Matchmaker mm)
    {
        _mm = mm;
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
            await _mm.Update();
        }
    }
}