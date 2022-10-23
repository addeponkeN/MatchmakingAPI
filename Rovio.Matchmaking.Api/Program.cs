using System.Reflection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using Rovio.Matchmaking.Api.Repositories;
using Rovio.Matchmaking.Api.Services;

namespace Rovio.Matchmaking.Api;

public class Program
{
    private static WebApplication _app;
    private static MatchmakingManager matchmakingManager;

    public static void Main(params string[] args)
    {
        matchmakingManager = new();

        BuildApplication(args);
        SetupApplication();
    }

    private static void SetupServices(IServiceCollection services)
    {
        services.AddSingleton<IClientRepository, ServerRepository>();
        services.AddSingleton<MatchmakingManager>(matchmakingManager);

        services.AddHostedService(_ =>
        {
            var mmService = new MatchmakingService();
            mmService.Init(matchmakingManager);
            return mmService;
        });

        services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc("v1", new OpenApiInfo() {Title = "Matchmaking API", Version = "v1"});
        
            x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
            });
        
            x.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme,
                        }
                    },
                    new List<string>()
                }
            });
        
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            x.IncludeXmlComments(xmlPath);
        
        });

        services.Configure<IISServerOptions>(opt => { opt.MaxRequestBodySize = 300_000_000; });

        services.Configure<KestrelServerOptions>(opt => { opt.Limits.MaxRequestBodySize = 300_000_000; });
    }

    private static void BuildApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        SetupServices(builder.Services);

        builder.WebHost.UseUrls("https://*:5000", "http://*:5001");

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