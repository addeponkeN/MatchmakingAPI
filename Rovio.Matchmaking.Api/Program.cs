using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Rovio.Matchmaking.Api.Repositories;
using Rovio.Matchmaking.Api.Services;
using Rovio.Matchmaking.Api.Settings;
using Rovio.Utility;

namespace Rovio.Matchmaking.Api;

public class Program
{
    private static WebApplication _app;
    private static MatchmakingManager matchmakingManager;

    public static void Main(params string[] args)
    {
        matchmakingManager = new();

#if RELEASE
        // Log.Enabled = false;
#endif
        BuildApplication(args);
        SetupApplication();
    }

    private static MatchmakingApiSettings LoadSettings()
    {
        const string settingsFilename = "matchmakingsettings.json";
        var path = Path.Combine(Directory.GetCurrentDirectory(), settingsFilename);

        MatchmakingApiSettings settings;
        
        if(!File.Exists(path))
        {
            settings = MatchmakingApiSettings.Default;
            JsonHelper.Save(path, settings);
        }
        else
        {
            settings = JsonHelper.Load<MatchmakingApiSettings>(path) 
                       ?? MatchmakingApiSettings.Default;
        }

        return settings;
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

        services.AddHealthChecks().AddCheck(
            name: "ready",
            check: () => new HealthCheckResult(HealthStatus.Healthy, "Matchmaker is ready"));

        services.Configure<IISServerOptions>(opt => { opt.MaxRequestBodySize = 300_000_000; });

        services.Configure<KestrelServerOptions>(opt => { opt.Limits.MaxRequestBodySize = 300_000_000; });
    }

    private static void BuildApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var settings = LoadSettings();
        
        SetupServices(builder.Services);

        builder.WebHost.UseUrls(settings.ListenUrls.ToArray());

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

        _app.UseRouting();

        _app.UseEndpoints(ep =>
        {
            ep.MapControllers();
            ep.MapHealthChecks("/hp/ready", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains("ready"),
                ResponseWriter = async (context, report) =>
                {
                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key, 
                            status = e.Value.Status.ToString(),
                            exception = e.Value.Exception != null ? e.Value.Exception.Message : "none",
                            duration = e.Value.Duration.ToString()
                        })
                    });
                    context.Request.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(result);
                }
            });
        });

        _app.Run();
    }
}
