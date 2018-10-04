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
                new GameTasks("Ettor"), //0
                new GameTasks("Tvåor"), //1
                new GameTasks("Treor"), //2
                new GameTasks("Fyror"), //3
                new GameTasks("Femmor"), //4
                new GameTasks("Sexor"), //5

                new GameTasks("Bonus", 50), //6 (ej valbar)

                new GameTasks("Par"), //7
                new GameTasks("Två par"), //8
                new GameTasks("Tretal"), //9
                new GameTasks("Fyrtal"), //10
                new GameTasks("Liten stege", 15), //11
                new GameTasks("Stor stege", 20), //12
                new GameTasks("Kåk"), //13
                new GameTasks("Chans"), //14
                new GameTasks("Yatzy", 50) //15
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

                    bool lastTry = false;

                    for (int försök = 1; försök <= 3; försök++)
                    {
                        if (försök == 3)
                            lastTry = true;

                        Task.PressEnter("slå tärningarna");

                        List<int> diceValues = new List<int>();
                        for (int j = 0; j < dice.Length; j++)
                        {
                            dice[j] = new Random().Next(1, 7);
                            diceValues.Add(dice[j]);

                            Console.WriteLine($"Tärning {j + 1} = {dice[j]}");
                        }
                        Console.WriteLine();

                        playersTasks = Program.CheckTasks(diceValues, playersTasks, i, lastTry);
                    }
                }
            }
        }

        private static Dictionary<User, List<GameTasks>> CheckTasks(List<int> rolledDice, Dictionary<User, List<GameTasks>> pTasks, int playerPos, bool lastTry)
        {
            var playersTasks = pTasks.ToList(); //splittar dictionariet med spelarna och deras listor med gametasks så att man kan komma åt värdena
            var currentPlayerTaskArray = playersTasks[playerPos].Value.ToArray(); //omvandlar List<Gametasks> i playersTasks(i 'game' funktionen) till en array

            var chooseTask = new Dictionary<ConsoleKey, GameTasks>
            {
                { ConsoleKey.D1, currentPlayerTaskArray[0] }, //Ettor
                { ConsoleKey.D2, currentPlayerTaskArray[1] }, //Tvåor
                { ConsoleKey.D3, currentPlayerTaskArray[2] }, //Treor
                { ConsoleKey.D4, currentPlayerTaskArray[3] }, //Fyror
                { ConsoleKey.D5, currentPlayerTaskArray[4] }, //Femmor
                { ConsoleKey.D6, currentPlayerTaskArray[5] }, //Sexor

                { ConsoleKey.D7, currentPlayerTaskArray[7] }, //Par
                { ConsoleKey.D8, currentPlayerTaskArray[8] }, //Två par
                { ConsoleKey.T, currentPlayerTaskArray[9] }, //Tretal
                { ConsoleKey.F, currentPlayerTaskArray[10] }, //Fyrtal
                { ConsoleKey.L, currentPlayerTaskArray[11] }, //Liten stege
                { ConsoleKey.S, currentPlayerTaskArray[12] }, //Stor stege
                { ConsoleKey.K, currentPlayerTaskArray[13] }, //Kåk
                { ConsoleKey.C, currentPlayerTaskArray[14] }, //Chans
                { ConsoleKey.Y, currentPlayerTaskArray[15] }, //Yatzy
            };

            var chooseTaskArray = chooseTask.ToArray();

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

            for (int taskNum = 0; taskNum < currentPlayerTaskArray.Length; taskNum++)
            {
                int chooseTaskNum = taskNum;

                if (taskNum >= 7)
                    chooseTaskNum = taskNum - 1;

                else if (taskNum == 6)
                {
                    if (currentPlayerTaskArray[taskNum].completed == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(currentPlayerTaskArray[taskNum].name + "\n");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (currentPlayerTaskArray[0].completed == true && currentPlayerTaskArray[1].completed == true &&
                            currentPlayerTaskArray[2].completed == true && currentPlayerTaskArray[3].completed == true &&
                            currentPlayerTaskArray[4].completed == true && currentPlayerTaskArray[5].completed == true &&
                            currentPlayerTaskArray[taskNum].completed == false) //kollar om spelaren har klarat ettor, tvåor o.s.v, samt att spelaren inte redan har klarat bonusen
                    {
                        currentPlayerTaskArray[taskNum].completed = true;
                        playersTasks[playerPos].Key.score += currentPlayerTaskArray[taskNum].score;

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(currentPlayerTaskArray[taskNum].name + "\n");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(currentPlayerTaskArray[taskNum].name + "\n");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

                if (currentPlayerTaskArray[taskNum].completed == true && taskNum != 6)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{chooseTaskArray[chooseTaskNum].Key} - {chooseTaskArray[chooseTaskNum].Value.name}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (currentPlayerTaskArray[taskNum].failed == true && taskNum != 6)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{chooseTaskArray[chooseTaskNum].Key} - {chooseTaskArray[chooseTaskNum].Value.name}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    if (taskNum == 0) //Ettor
                    {
                        if (antalEttor >= 3)
                            currentPlayerTaskArray[taskNum].available = true;
                    }
                    else if (taskNum == 1) //Tvåor
                    {
                        if (antalTvåor >= 3)
                            currentPlayerTaskArray[taskNum].available = true;
                    }
                    else if (taskNum == 2) //Treor
                    {
                        if (antalTreor >= 3)
                            currentPlayerTaskArray[taskNum].available = true;
                    }
                    else if (taskNum == 3) //Fyror
                    {
                        if (antalFyror >= 3)
                            currentPlayerTaskArray[taskNum].available = true;
                    }
                    else if (taskNum == 4) //Femmor
                    {
                        if (antalFemmor >= 3)
                            currentPlayerTaskArray[taskNum].available = true;
                    }
                    else if (taskNum == 5) //Sexor
                    {
                        if (antalSexor >= 3)
                            currentPlayerTaskArray[taskNum].available = true;
                    }

                    //Bonus-tasken kollas innan

                    else if (taskNum == 7) //Par
                    {
                        if (listOfDiceNums.Contains(2))
                            currentPlayerTaskArray[taskNum].available = true;
                    }
                    else if (taskNum == 8) // Två par
                    {
                        //FIXA WALLAAAAAA!!!!!!!!!
                    }
                    else if (taskNum == 9) //Tretal
                    {
                        if (listOfDiceNums.Contains(3) || listOfDiceNums.Contains(4) || listOfDiceNums.Contains(5))
                            currentPlayerTaskArray[taskNum].available = true;
                    }
                    else if (taskNum == 10) //Fyrtal
                    {
                        if (listOfDiceNums.Contains(4) || listOfDiceNums.Contains(5))
                            currentPlayerTaskArray[taskNum].available = true;
                    }
                    else if (taskNum == 11) //Liten stege
                    {
                        if (antalEttor == 1 && antalTvåor == 1 &&
                            antalTreor == 1 && antalFyror == 1 && antalFemmor == 1)
                            currentPlayerTaskArray[taskNum].available = true;
                    }
                    else if (taskNum == 12) //Stor stege
                    {
                        if (antalTvåor == 1 && antalTreor == 1 &&
                            antalFyror == 1 && antalFemmor == 1 && antalSexor == 1)
                            currentPlayerTaskArray[taskNum].available = true;
                    }
                    else if (taskNum == 13) //Kåk
                    {
                        if (listOfDiceNums.Contains(3) && listOfDiceNums.Contains(2))
                            currentPlayerTaskArray[taskNum].available = true;
                    }
                    else if (taskNum == 14) //Chans
                    {
                        currentPlayerTaskArray[taskNum].available = true;
                    }
                    else if (taskNum == 15) //Yatzy
                    {
                        if (listOfDiceNums.Contains(5))
                            currentPlayerTaskArray[taskNum].available = true;
                    }

                    //skriv ut om den kan väljas (vit-färg) eller om den inte kan (grå-färg)
                    if (currentPlayerTaskArray[taskNum].available == true)
                        Console.WriteLine($"{chooseTaskArray[chooseTaskNum].Key} - {chooseTaskArray[chooseTaskNum].Value.name}");
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine($"{chooseTaskArray[chooseTaskNum].Key} - {chooseTaskArray[chooseTaskNum].Value.name}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            bool aTaskIsAvailable = false;

            foreach (GameTasks task in currentPlayerTaskArray)
            {
                if (task.available == true)
                    aTaskIsAvailable = true;
            }

            ConsoleKey chosenTaskKey;
            if (aTaskIsAvailable == false && lastTry == true) //då måste användaren stryka en task
            {
                Console.WriteLine("\nDu har inte en enda kombination för att klara en uppgift.");
                Console.WriteLine("Du måste nu stryka en uppgift.\n");

                Console.WriteLine("\nVilken uppgift vill du stryka?");
                Console.WriteLine("Klicka på motsvarande tangent för att välja.");

                chosenTaskKey = Console.ReadKey(true).Key;

                foreach (KeyValuePair<ConsoleKey, GameTasks> task in chooseTask)
                {
                    if (chosenTaskKey == task.Key)
                    {

                    }
                }
            }
            else if (aTaskIsAvailable == true)
            Program.CalcTaskScore(listOfDiceNums);
        }

        private static void CalcTaskScore(List<int> listOfDiceNums)
        {

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