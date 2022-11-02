using Rovio.Matchmaking.Client;
using Rovio.Matchmaking.Models;
using Rovio.Utility;

namespace MatchmakingClientExample
{
    internal static class Program
    {
        private static int _idPool = 1;
        private static Continents[] _allContinents = Enum.GetValues<Continents>();

        private static MatchmakingClient _client;
        private static List<Player> _players = new();

        private static async Task Main(params string[] args)
        {
            var address = "localhost";
            var port = 5000;
            var continent = Continents.EU;

            _client = new MatchmakingClient(continent, address, port);

            await ChooseGameService();
            await ApiCallSimulator();
        }

        private static async Task ChooseGameService()
        {
            Log.Debug("Hello, Rovio!");
            Log.Debug("Press <enter> to start");
            Console.ReadLine();

            chooseService:

            Console.Clear();

            var chosenService = ConsoleExtended.RequestChoice(
                "Select Game Server Type",
                new GameService()
                    {GameName = "Angry Birds", GameServiceId = Guid.Parse("af3c0213-538e-4629-a725-ea56f8a3acec")},
                new GameService()
                    {GameName = "Bad Piggies", GameServiceId = Guid.Parse("29f7cf48-8657-4849-9b26-9d19d81219f9")},
                new GameService()
                    {GameName = "World Quest", GameServiceId = Guid.Parse("4c2afede-168c-44b0-a49d-07ed20e6480d")}
            );

            Log.Debug();
            Log.Debug($"Registering as '{chosenService.GameName}'...");

            var response = await _client.RegisterServer(new Server
            {
                GameServiceId = chosenService.GameServiceId,
            });

            if(response.IsSuccessStatusCode)
            {
                var validation = _client.ValidatedServer;
                Log.Debug($"Registering complete!");
                Log.Debug($"Your token '{validation.ServerId}'");
                Log.Debug();
                Log.Debug($"Press <enter> to continue");
                Console.ReadLine();
            }
            else
            {
                Log.Debug($"Registering failed! :(");
                Log.Debug(response.StatusCode.ToStringEnum());
                Log.Debug();
                Log.Debug($"Press <enter> to retry");
                Console.ReadLine();
                goto chooseService;
            }
        }

        private static async Task ApiCallSimulator()
        {
            while(true)
            {
                int index = ConsoleExtended.RequestChoice(
                    $"Matchmaking API - Your token: '{_client.ValidatedServer.ServerId}'\n\nChoose API call\n",
                    "Add player to matchmaking",
                    "Add 10 - 1,000 random players to matchmaking",
                    "Add 1,000 - 100,000 random players to matchmaking",
                    "Add 100,000 - 1,500,000 random players to matchmaking",
                    "Remove random player from matchmaking",
                    "Add an ongoing session that is missing player(s)",
                    "Get ready sessions",
                    "Get ready sessions and save to file (..\\readysessions.json)",
                    "Get ready ongoing sessions"
                );

                index++;

                Log.Debug();

                switch(index)
                {
                    //  ADD PLAYER TO MATCHMAKING
                    case 1:
                    {
                        Log.Debug("Adding player..");
                        var model = GeneratePlayer();
                        var response = await _client.AddPlayer(model);
                        Log.Debug(response.StatusCode.ToString());
                        if(!response.IsSuccessStatusCode)
                        {
                            Log.Debug(response.RequestMessage?.RequestUri?.ToString());
                        }

                        break;
                    }

                    
                    //  ADD RANDOM AMOUNT OF PLAYERS TO MATCHMAKING
                    case 2:
                    {
                        int count = Random.Shared.Next(10, 1000);

                        await AddPlayers(count);

                        break;
                    }


                    //  ADD RANDOM AMOUNT OF PLAYERS TO MATCHMAKING
                    case 3:
                    {
                        int count = Random.Shared.Next(1_000, 100_00);

                        await AddPlayers(count);

                        break;
                    }


                    //  ADD RANDOM AMOUNT OF PLAYERS TO MATCHMAKING
                    case 4:
                    {
                        int count = Random.Shared.Next(100_000, 1_500_000);

                        await AddPlayers(count);

                        break;
                    }


                    //  REMOVE RANDOM PLAYER FROM MATCHMAKING
                    case 5:
                    {
                        if(_players.Count <= 0)
                        {
                            Log.Debug("No players");
                            break;
                        }

                        var randIndex = Random.Shared.Next(0, _players.Count);
                        var player = _players[randIndex];
                        _players.RemoveAt(randIndex);

                        var res = await _client.RemovePlayer(player.UniqueId);

                        Log.Debug(res.StatusCode.ToString());

                        break;
                    }

                    
                    //  ADD AN ONGOING SESSION TO MATCHMAKING (MISSING PLAYERS)
                    case 6:
                    {
                        Continents continent = Continents.EU;
                        int missingPlayerCount = Random.Shared.Next(1, 4);

                        var match = new OngoingSessions()
                        {
                            Continent = continent,
                            MissingPlayersCount = missingPlayerCount,
                        };

                        var matchResponse = await _client.AddOngoingMatch(match);

                        if(matchResponse.IsSuccessStatusCode)
                        {
                            Log.Debug("Added ongoing match");
                        }
                        else
                        {
                            Log.Debug(matchResponse.StatusCode.ToStringEnum());
                        }

                        break;
                    }

                    
                    //  GET READY SESSIONS
                    case 7:
                    {
                        var sessions = await _client.GetReadySessions();

                        int sessionCount = sessions == null ? 0 : sessions.Sessions.Count();

                        if(sessionCount > 0)
                        {
                            // ClearPlayers(sessions.Sessions);
                        }

                        Log.Debug($"Got '{sessionCount}' ready sessions");
                        break;
                    }


                    //  GET READY SESSIONS & SAVE TO FILE (JSON)
                    case 8:
                    {
                        var readySessions = await _client.GetReadySessions();

                        int sessionCount = readySessions == null ? 0 : readySessions.Sessions.Count();

                        if(sessionCount > 0)
                        {
                            // ClearPlayers(readySessions.Sessions);
                        }

                        string path = Path.Combine(Directory.GetCurrentDirectory(), "readysessions.json");
                        JsonHelper.Save(path, readySessions);

                        Log.Debug($"Saved '{sessionCount}' ReadySessions to {path}");
                        break;
                    }


                    //  GET READY ONGOING SESSIONS
                    case 9:
                    {
                        var ongoingSessions = await _client.GetReadyOngoingSessions();

                        int sessionCount = ongoingSessions == null ? 0 : ongoingSessions.Sessions.Count();

                        if(sessionCount > 0)
                        {
                            // ClearPlayers(ongoingSessions.Sessions);
                        }

                        Log.Debug($"Got '{sessionCount}' ready ongoing sessions");
                        break;
                    }
                }

                Log.Debug("<Enter> to continue");
                Console.ReadLine();
            }
        }

        private static void ClearPlayers(IEnumerable<Session> sessions)
        {
            foreach(var ses in sessions)
            foreach(var player in ses.Players)
                for(int i = 0; i < _players.Count; i++)
                {
                    if(player.UniqueId.Equals(_players[i].UniqueId))
                        _players.RemoveAt(i--);
                }
        }

        private static async Task AddPlayers(int count)
        {
            var list = new List<Player>(count);

            Log.Debug($"Generating '{count}' players...");

            for(int i = 0; i < count; i++)
            {
                list.Add(GeneratePlayer());
            }

            var group = new PlayerGroup()
            {
                Players = list
            };

            Log.Debug($"Adding '{count} players...");

            var response = await _client.AddPlayers(group);

            if(response.IsSuccessStatusCode)
            {
                Log.Debug($"'{count}' players added to matchmaking!");
            }
            else
            {
                Log.Debug($"{response.StatusCode}");
            }
        }

        private static Player GeneratePlayer()
        {
            var model = CreatePlayer(
                // $"Goofy{Random.Shared.Next(int.MaxValue)}",
                GetRandomContinent(),
                Random.Shared.Next(1, 10));
            return model;
        }

        private static Player CreatePlayer(Continents continent, int rank)
        {
            var player = new Player()
            {
                UniqueId = Guid.NewGuid(),
                Continent = continent,
                Rank = rank
            };
            _players.Add(player);
            return player;
        }

        private static Continents GetRandomContinent()
        {
            return Continents.EU;
            return _allContinents[Random.Shared.Next(1, _allContinents.Length)];
        }
    }
}