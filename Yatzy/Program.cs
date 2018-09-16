using System;
using System.Collections.Generic;
using EliasLibrary;

namespace Yatzy
{
    internal class GameTasks
    {
        public bool completed = false;
        public bool failed = false;
        public bool available = false;

        public string name;
        public int score;

        public GameTasks(string name) { this.name = name; }

        public GameTasks(string name, int score)
        {
            this.name = name;
            this.score = score;
        }
    }

    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("-Yatzy-");
            Task.PressEnter("börja");

            List<User> YatzyPlayers = new List<User>();
            int maxPlayers = 5;

            List<GameTasks> YatzyGameTasks = Program.AddTasks();

            while (true)
            {
                Console.Clear();
                Console.Write("Tryck [1] för att skapa en användare");
                if (YatzyPlayers.Count > 0)
                    Console.Write(" | Tryck [2] för att starta spelet");

                ConsoleKey answer = Console.ReadKey(true).Key;

                if (answer == ConsoleKey.D1)
                {
                    User.CreateUser(YatzyPlayers);

                    Program.WriteBoard(YatzyPlayers, "Registrerade spelare");

                    if (YatzyPlayers.Count == maxPlayers)
                    {
                        Console.WriteLine("Max antal spelare har uppnåtts!");
                        Task.PressEnter("börja spelet");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Det finns {maxPlayers - YatzyPlayers.Count} platser kvar.");
                        Task.PressEnter("gå vidare");
                    }
                }
                else if (answer == ConsoleKey.D2 && YatzyPlayers.Count > 0)
                    break;
            }
            Console.Clear();

            Program.Game(YatzyPlayers, YatzyGameTasks);
        }

        private static List<GameTasks> AddTasks()
        {
            List<GameTasks> tasks = new List<GameTasks>
            {
                new GameTasks("Ettor"),
                new GameTasks("Tvåor"),
                new GameTasks("Treor"),
                new GameTasks("Fyror"),
                new GameTasks("Femmor"),
                new GameTasks("Sexor"),

                new GameTasks("Bonus", 50),

                new GameTasks("Par"),
                new GameTasks("Två par"),
                new GameTasks("Tretal"),
                new GameTasks("Fyrtal"),
                new GameTasks("Liten stege", 15),
                new GameTasks("Stor stege", 20),
                new GameTasks("Kåk"),
                new GameTasks("Chans"),
                new GameTasks("Yatzy", 50)
            };

            return tasks;
        }

        private static void Game(List<User> playerList, List<GameTasks> taskList)
        {
            var dice = new int[5];
            var playersTasks = new Dictionary<User, List<GameTasks>>();

            foreach (User player in playerList)
                playersTasks.Add(player, taskList);

            for (int round = 1; round <= 15; round++)
            {
                Console.WriteLine($"Runda {round} av 15.");
                Task.PressEnter("börja rundan");

                foreach (User player in playerList)
                {
                    if (player.name.EndsWith("s"))
                        Console.WriteLine($"Nu är det {player.name} tur att kasta!");
                    else
                        Console.WriteLine($"Nu är det {player.name}s tur att kasta!");

                    for (int försök = 1; försök <= 3; försök++)
                    {
                        Task.PressEnter("slå tärningarna");

                        List<int> diceValues = new List<int>();
                        for (int i = 0; i < dice.Length; i++)
                        {
                            dice[i] = new Random().Next(1, 7);
                            diceValues.Add(dice[i]);

                            Console.WriteLine($"Tärning {i + 1} = {dice[i]}");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }

        private static void WriteBoard(List<User> playerList, string board)
        {
            Console.Clear();

            Console.WriteLine(board + ":\n");
            int i = 1;
            foreach (User player in playerList)
            {
                Console.WriteLine($"{i}. {player.name} - {player.score}");
                i++;
            }
            Console.WriteLine();
        }
    }
}