using Rovio.Matchmaking.Models;
using Rovio.Utility;

namespace Rovio.Matchmaking.Api.Repositories;

public class ServerRepository : IClientRepository
{
    private Dictionary<Guid, GameServiceModel> _registeredServices = new();
    private Dictionary<Guid, ValidatedServerModel> _validatedServers = new();

    private MatchmakingManager _mm;
    
    public ServerRepository(MatchmakingManager manager)
    {
        _mm = manager;
        LoadServices();
    }

    private void LoadServices()
    {
        //  temporary - get from somewhere else (file/database)
        AddGameService("Angry Birds", "af3c0213-538e-4629-a725-ea56f8a3acec");
        AddGameService("Bad Piggies", "29f7cf48-8657-4849-9b26-9d19d81219f9");
        AddGameService("World Quest", "4c2afede-168c-44b0-a49d-07ed20e6480d");
    }

    private void AddGameService(string name, string id)
    {
        AddService(new GameServiceModel()
        {
            Id = Guid.Parse(id),
            Name = name
        });
    }

    private void AddService(GameServiceModel service)
    {
        Log.Debug($"Added game service: {service.Name} ({service.Id})");
        _mm.Add(new Matchmaker(service.Id));
        _registeredServices.Add(service.Id, service);
    }

    private ValidatedServerModel CreateValidatedServer(Guid gameServiceId)
    {
        return new ValidatedServerModel()
        {
            ServerId = Guid.NewGuid(),
            GameServiceId = gameServiceId
        };
    }

    private bool IsIdValid(in Guid guid)
    {
        return !guid.Equals(Guid.Empty);
    }

    private bool IsGameServiceRegistered(Guid serviceId)
    {
        return _registeredServices.ContainsKey(serviceId);
    }

//  CLIENT REPOSITORY FUNCTIONS

    public bool TryRegisterServer(ServerModel server, out ValidatedServerModel? validatedServer)
    {
        // Log.Debug($"registering server: {server.GameServiceId}");
        validatedServer = null;

        if(!IsIdValid(server.GameServiceId))
        {
            Log.Warning($"Invalid game service id ({server.GameServiceId})");
            return false;
        }

        if(!_registeredServices.TryGetValue(server.GameServiceId, out var gameService))
        {
            Log.Warning($"Game service is not registered ({server.GameServiceId})");
            return false;
        }

        validatedServer = CreateValidatedServer(server.GameServiceId);
        _validatedServers.Add(validatedServer.ServerId, validatedServer);

        Log.Debug($"Server registered to '{gameService.Name}' with id '{validatedServer.ServerId}'");

        return true;
    }

    public bool TryGetGameServiceId(Guid clientToken, out Guid gameServiceId)
    {
        if(_validatedServers.TryGetValue(clientToken, out var serverModel))
        {
            gameServiceId = serverModel.GameServiceId;
            return true;
        }

        gameServiceId = Guid.Empty;
        return false;
    }

    public bool IsTokenValid(Guid id)
    {
        return _validatedServers.ContainsKey(id);
    }
}