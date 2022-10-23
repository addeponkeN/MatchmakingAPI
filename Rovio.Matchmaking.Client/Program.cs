using Rovio.Matchmaking.Models;
using Rovio.Utility;

namespace Rovio.Matchmaking.Client
{
    internal static class Program
    {
        private static int _idPool = 1;
        private static Continents[] _allContinents = Enum.GetValues<Continents>();

        private static MatchmakingClient _client;

        private static async Task Main(params string[] args)
        {
            string address = "localhost";
            int port = 5000;

            _client = new MatchmakingClient(address, port);

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
                new GameServiceModel() {Name = "Angry Birds", Id = Guid.Parse("af3c0213-538e-4629-a725-ea56f8a3acec")},
                new GameServiceModel() {Name = "Bad Piggies", Id = Guid.Parse("29f7cf48-8657-4849-9b26-9d19d81219f9")},
                new GameServiceModel() {Name = "World Quest", Id = Guid.Parse("4c2afede-168c-44b0-a49d-07ed20e6480d")}
            );

            Log.Debug();
            Log.Debug($"Registering as '{chosenService.Name}'...");

            var response = await _client.RegisterServer(new ServerModel
            {
                GameServiceId = chosenService.Id,
            });

            if(response == null || response.IsSuccessStatusCode)
            {
                var validation = _client.GetServerInfo;
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
                    $"Matchmaking API - Your token: '{_client.GetServerInfo.ServerId}'\n\nChoose API call\n",
                    "Add player to matchmaking",
                    "Add specified player to matchmaking",
                    "Add an ongoing session that is missing player(s)",
                    "Add random amount of players to matchmaking",
                    "Get ready sessions",
                    "Get ready sessions and save to file (..\\readysessions.json)");

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

                    
                    //  ADD SPECIFIED PLAYER TO MATCHMAKING
                    case 2:
                    {
                        string name;
                        Continents continent;

                        name = ConsoleExtended.RequestString("Name: ");
                        if(string.IsNullOrEmpty(name))
                            name = "Donald";

                        continent = ConsoleExtended.RequestChoice(
                            "Continent:",
                            Continents.EU, Continents.NA, Continents.OC);

                        var model = CreatePlayer(name, continent, Random.Shared.Next(1, 10));

                        var response = await _client.AddPlayer(model);
                        Log.Debug(response.StatusCode.ToString());

                        break;
                    }


                    //  ADD AN ONGOING SESSION TO MATCHMAKING (MISSING PLAYERS)
                    case 3:
                    {
                        Continents continent = Continents.EU;
                        int missingPlayerCount = Random.Shared.Next(1, 4);
                        
                        break;
                    }
                    
                    
                    //  ADD RANDOM AMOUNT OF PLAYERS TO MATCHMAKING
                    case 4:
                    {
                        int count = Random.Shared.Next(1_000_000, 2_000_000);
                        // count = 10_000;

                        var list = new List<PlayerModel>(count);

                        Log.Debug($"Generating '{count}' players");

                        for(int i = 0; i < count; i++)
                        {
                            list.Add(GeneratePlayer());
                        }

                        var group = new PlayerGroupModel()
                        {
                            Players = list
                        };

                        var response = await _client.AddPlayers(group);

                        if(response.IsSuccessStatusCode)
                        {
                            Log.Debug($"'{count}' players added to matchmaking!");
                        }

                        break;
                    }

                    
                    //  GET READY SESSIONS
                    case 5:
                    {
                        var sessions = await _client.GetReadySessions();

                        int sessionCount = sessions == null ? 0 : sessions.Sessions.Count();

                        Log.Debug($"Got '{sessionCount}' ready sessions");
                        break;
                    }

                    
                    //  GET READY SESSIONS & SAVE TO FILE (JSON)
                    case 6:
                    {
                        var readySessions = await _client.GetReadySessions();

                        int sessionCount = readySessions == null ? 0 : readySessions.Sessions.Count();

                        string path = Path.Combine(Directory.GetCurrentDirectory(), "readysessions.json");
                        JsonHelper.Save(path, readySessions);

                        Log.Debug($"Saved '{sessionCount}' ReadySessions to {path}");
                        break;
                    }
                }

                Log.Debug("<Enter> to continue");
                Console.ReadLine();
            }
        }

        private static PlayerModel GeneratePlayer()
        {
            var model = CreatePlayer(
                $"Goofy{Random.Shared.Next(int.MaxValue)}",
                GetRandomContinent(),
                Random.Shared.Next(1, 10));
            return model;
        }

        private static PlayerModel CreatePlayer(string name, Continents continent, int rank)
        {
            var model = new PlayerModel()
            {
                Key = _idPool++,
                Name = name,
                Continent = continent,
                Rank = rank
            };
            return model;
        }

        private static Continents GetRandomContinent()
        {
            return Continents.EU;
            return _allContinents[Random.Shared.Next(1, _allContinents.Length)];
        }
    }
}