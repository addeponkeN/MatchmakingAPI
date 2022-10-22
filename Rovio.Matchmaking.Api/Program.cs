using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Rovio.Matchmaking.Api.Services;

namespace Rovio.Matchmaking.Api;

public class Program
{
    private static WebApplication _app;

    public static Matchmaker Matchmaker;

    public static void Main(params string[] args)
    {
        Matchmaker = new();

        BuildApplication(args);
        SetupApplication();
    }

    private static void SetupServices(IServiceCollection services)
    {
        // services.AddSingleton<IMatchmaker, Matchmaker>();

        services.AddHostedService(_ =>
        {
            var mmService = new MatchmakingService();
            mmService.Init(Matchmaker);
            return mmService;
        });

        services.Configure<IISServerOptions>(opt =>
        {
            opt.MaxRequestBodySize = 100_000_000;
        });
        
        services.Configure<KestrelServerOptions>(opt =>
        {
            opt.Limits.MaxRequestBodySize = 100_000_000;
        });
    }

    private static void BuildApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        SetupServices(builder.Services);
        
        builder.WebHost.UseUrls("https://*:5000");

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        _app = builder.Build();
    }

    private static void SetupApplication()
    {
        if(_app.Environment.IsDevelopment())
        {
            _app.UseSwagger();
            _app.UseSwaggerUI();
        }

        _app.UseHttpsRedirection();
        _app.UseAuthorization();
        _app.MapControllers();

        _app.Run();
    }
}