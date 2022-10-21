namespace Rovio.Matchmaking.Api.Services;

public class MatchmakeService : BackgroundService
{
    private int _frequency;
    private Matchmaker _mm;

    public void Init(Matchmaker mm)
    {
        _mm = mm;
    }
    
    //  todo : load from config
    public void LoadSettings()
    {
        _frequency = 1000;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LoadSettings();
        while(!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_frequency);
            await _mm.Matchmake();
        }
    }
}