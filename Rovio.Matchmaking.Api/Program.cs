using Rovio.Matchmaking.Api.Services;

namespace Rovio.Matchmaking.Api;

public class Program
{
    private static WebApplication _app;

    public static void Main(params string[] args)
    {
        BuildApplication(args);
        SetupApplication();
    }

    private static void BuildApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
        builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHostedService<MatchmakeService>();

        _app = builder.Build();
    }

    private static void SetupApplication()
    {
// Configure the HTTP request pipeline.
        if(_app.Environment.IsDevelopment())
        {
            _app.UseSwagger();
            _app.UseSwaggerUI();
        }

        var mm = _app.Services.GetService<MatchmakeService>();

        var settings = new MatchmakingSettings();
        mm.Init(new Matchmaker(settings));

        _app.UseHttpsRedirection();

        _app.UseAuthorization();

        _app.MapControllers();

        _app.Run();
    }
}