using Rovio.Matchmaking.Models;

namespace Rovio.Matchmaking.Api.Repositories;

public interface IClientRepository
{
    bool TryRegisterServer(ServerModel server, out ValidatedServerModel? validatedServer);
    bool TryGetGameServiceId(Guid clientToken, out Guid gameServiceId);
    bool IsTokenValid(Guid serverId);
}