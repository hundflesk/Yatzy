using System;
using System.Linq;
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
                new GameTasks("Ettor"), //1
                new GameTasks("Tvåor"), //2
                new GameTasks("Treor"), //3
                new GameTasks("Fyror"), //4
                new GameTasks("Femmor"), //5
                new GameTasks("Sexor"), //6

                new GameTasks("Bonus", 50), //7

                new GameTasks("Par"), //8
                new GameTasks("Två par"), //9
                new GameTasks("Tretal"), //10
                new GameTasks("Fyrtal"), //11
                new GameTasks("Liten stege", 15), //12
                new GameTasks("Stor stege", 20), //13
                new GameTasks("Kåk"), //14
                new GameTasks("Chans"), //15
                new GameTasks("Yatzy", 50) //16
            };

            return tasks;
        }

        private static void Game(List<User> playerList, List<GameTasks> taskList)
        {
            var dice = new int[5];
            var playerArray = playerList.ToArray();
            var playersTasks = new Dictionary<User, List<GameTasks>>();

            foreach (User player in playerList)
                playersTasks.Add(player, taskList);

            for (int round = 1; round <= 15; round++)
            {
                Console.WriteLine($"Runda {round} av 15.");
                Task.PressEnter("börja rundan");

                for (int i = 0; i < playerArray.Length; i++)
                {
                    if (playerArray[i].name.EndsWith("s"))
                        Console.WriteLine($"Nu är det {playerArray[i].name} tur att kasta!");
                    else
                        Console.WriteLine($"Nu är det {playerArray[i].name}s tur att kasta!");

                    for (int försök = 1; försök <= 3; försök++)
                    {
                        Task.PressEnter("slå tärningarna");

                        List<int> diceValues = new List<int>();
                        for (int j = 0; j < dice.Length; j++)
                        {
                            dice[j] = new Random().Next(1, 7);
                            diceValues.Add(dice[j]);

                            Console.WriteLine($"Tärning {j + 1} = {dice[j]}");
                        }
                        Console.WriteLine();

                        Program.CheckTasks(diceValues, playersTasks, i);
                    }
                }
            }
        }

        private static Dictionary<User, List<GameTasks>> CheckTasks(List<int> rolledDice, Dictionary<User, List<GameTasks>> pTasks, int playerPos)
        {
            var playersTasks = pTasks.ToList();
            var taskArray = playersTasks[playerPos].Value.ToArray();

            int antalEttor = 0;
            int antalTvåor = 0;
            int antalTreor = 0;
            int antalFyror = 0;
            int antalFemmor = 0;
            int antalSexor = 0;

            foreach (int diceNum in rolledDice)
            {
                if (diceNum == 1)
                    antalEttor += 1;

                else if (diceNum == 2)
                    antalTvåor += 1;

                else if (diceNum == 3)
                    antalTreor += 1;

                else if (diceNum == 4)
                    antalFyror += 1;

                else if (diceNum == 5)
                    antalFemmor += 1;

                else if (diceNum == 6)
                    antalSexor += 1;
            }

            List<int> listOfDiceNums = new List<int>
            {
                antalEttor,
                antalTvåor,
                antalTreor,
                antalFyror,
                antalFemmor,
                antalSexor
            };

            for (int i = 0; i < taskArray.Length; i++)
            {
                if (taskArray[i].completed == true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[tangent] - {taskArray[i].name}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (taskArray[i].failed == true)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[tangent] - {taskArray[i].name}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    if (i == 0) //Ettor
                    {
                        if (antalEttor >= 3)
                            taskArray[i].available = true;
                    }
                    else if (i == 1) //Tvåor
                    {
                        if (antalTvåor >= 3)
                            taskArray[i].available = true;
                    }
                    else if (i == 2) //Treor
                    {
                        if (antalTreor >= 3)
                            taskArray[i].available = true;
                    }
                    else if (i == 3) //Fyror
                    {
                        if (antalFyror >= 3)
                            taskArray[i].available = true;
                    }
                    else if (i == 4) //Femmor
                    {
                        if (antalFemmor >= 3)
                            taskArray[i].available = true;
                    }
                    else if (i == 5) //Sexor
                    {
                        if (antalSexor >= 3)
                            taskArray[i].available = true;
                    }
                    else if (i == 6) //Par
                    {
                        if (listOfDiceNums.Contains(2))
                            taskArray[i].available = true;
                    }
                    else if (i == 7) //Två par
                    {

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