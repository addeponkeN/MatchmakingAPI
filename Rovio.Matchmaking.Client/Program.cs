using Rovio.Matchmaking.Models;
using Rovio.Utility;

namespace Rovio.Matchmaking.Client
{
    class Program
    {
        private static int _idPool = 1;
        private static Continents[] _allContinents = Enum.GetValues<Continents>();

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
                // Name = name,
                Continent = continent,
                Rank = rank
            };
            return model;
        }

        private static Continents GetRandomContinent()
        {
            return _allContinents[Random.Shared.Next(1, _allContinents.Length)];
        }

        static void Main(params string[] args)
        {
            string address = "localhost";
            int port = 5000;

            var client = new MatchmakingClient(address, port);

            Console.WriteLine("Hello, Rovio!");
            Console.WriteLine("Press enter to matchmake");
            Console.Clear();

            while(true)
            {
                Console.WriteLine("1) Add player");
                Console.WriteLine("2) Add specified player");
                Console.WriteLine("3) Add many random players");
                Console.WriteLine("4) Get ready sessions");
                Console.WriteLine("5) Get ready sessions and save to file");

                var input = Console.ReadLine();

                switch(input)
                {
                    case "1":
                    {
                        Task.Run(async () =>
                        {
                            Log.Debug("adding player..");
                            var model = GeneratePlayer();
                            var response = await client.AddPlayer(model);
                            Log.Debug(response.StatusCode.ToString());
                            if(!response.IsSuccessStatusCode)
                            {
                                Log.Debug(response.RequestMessage?.RequestUri?.ToString());
                            }
                        });

                        break;
                    }

                    case "2":
                    {
                        Console.WriteLine("Name: ");
                        Continents continent = Continents.EU;
                        string name = Console.ReadLine();

                        if(string.IsNullOrEmpty(name))
                            name = "Donald";

                        Console.WriteLine("Continent: ");
                        Console.WriteLine("1) eu");
                        Console.WriteLine("2) na");
                        Console.WriteLine("3) oc");

                        string continentInput = Console.ReadLine();

                        switch(continentInput)
                        {
                            case "1":
                                continent = Continents.EU;
                                break;
                            case "2":
                                continent = Continents.NA;
                                break;
                            case "3":
                                continent = Continents.OC;
                                break;
                        }

                        var model = CreatePlayer(name, continent, Random.Shared.Next(1, 10));

                        Task.Run(async () =>
                        {
                            var response = await client.AddPlayer(model);
                            Log.Debug(response.StatusCode.ToString());
                        });

                        break;
                    }

                    case "3":
                    {
                        Task.Run(async () =>
                        {
                            int count = Random.Shared.Next(1, 500_000);
                            count = 100_00;

                            var list = new List<PlayerModel>(count);

                            Console.WriteLine($"Generating '{count}' players");

                            for(int i = 0; i < count; i++)
                            {
                                list.Add(GeneratePlayer());
                            }

                            var group = new PlayerGroupModel()
                            {
                                Players = list
                            };

                            var response = await client.AddPlayers(group);

                            if(response.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"'{count}' players added!");
                            }
                        });

                        break;
                    }

                    case "4":
                    {
                        Task.Run(async () =>
                        {
                            var sessions = await client.GetReadySessions();

                            int sessionCount = sessions == null ? 0 : sessions.Sessions.Count();

                            Console.WriteLine($"got '{sessionCount}' ready sessions");
                        });
                        break;
                    }

                    case "5":
                    {
                        Task.Run(async () =>
                        {
                            var readySessions = await client.GetReadySessions();

                            int sessionCount = readySessions == null ? 0 : readySessions.Sessions.Count();

                            string path = $"{Directory.GetCurrentDirectory()}/readysessions.json";
                            JsonHelper.Save(path, readySessions);

                            Log.Debug($"got '{sessionCount}' ready sessions");
                            Log.Debug($"Saved ReadySessions to {path}");
                        });
                        break;
                    }
                }
            }
        }
    }
}