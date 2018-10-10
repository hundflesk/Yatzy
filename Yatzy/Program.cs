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
        public ConsoleKey key;

        public GameTasks(string name, int score)
        { //Används bara för Bonus
            this.name = name;
            this.score = score;
        }

        public GameTasks(string name, ConsoleKey key)
        { //Används för tasks som kan ha olika poäng
            this.name = name;
            this.key = key;
        }

        public GameTasks(string name, ConsoleKey key, int score)
        { //Används för tasks som har bestämda poäng
            this.name = name;
            this.score = score;
            this.key = key;
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
                new GameTasks("Ettor", ConsoleKey.D1), //0 Ettor
                new GameTasks("Tvåor", ConsoleKey.D2), //1 Tvåor
                new GameTasks("Treor", ConsoleKey.D3), //2 Treor
                new GameTasks("Fyror", ConsoleKey.D4), //3 Fyror
                new GameTasks("Femmor", ConsoleKey.D5), //4 Femmor
                new GameTasks("Sexor", ConsoleKey.D6), //5 Sexor

                new GameTasks("Bonus", 50), //6 Bonus (ej valbar), har en bestämnd score

                new GameTasks("Par", ConsoleKey.D7), //7 Par
                new GameTasks("Två par", ConsoleKey.D8), //8 Två par
                new GameTasks("Tretal", ConsoleKey.T), //9 Tretal
                new GameTasks("Fyrtal", ConsoleKey.F), //10 Fyrtal
                new GameTasks("Liten stege", ConsoleKey.L, 15), //11 Liten stege, har en bestämd score
                new GameTasks("Stor stege",ConsoleKey.S, 20), //12 Stor stege, har en bestämd score
                new GameTasks("Kåk", ConsoleKey.K), //13 Kåk
                new GameTasks("Chans", ConsoleKey.C), //14 Chans
                new GameTasks("Yatzy", ConsoleKey.Y, 50) //15 Yatzy, har en bestämd score
            };
            return tasks;
        }

        private static void Game(List<User> playerList, List<GameTasks> taskList)
        {
            var dice = new int[5];
            var playerArray = playerList.ToArray();
            var spelTabell = new Dictionary<User, List<GameTasks>>(); //speltabellen är själva pappret med alla namn och deras poäng

            //ger en individuell lista till varje person, alla ska ha en egen lista
            //enklare sagt: man skriver upp namnen på tabellen (IRL)
            foreach (User player in playerArray)
                spelTabell.Add(player, taskList);

            for (int round = 1; round <= 15; round++) //denna loop sköter hela spelet, när den är slut har man kört igenom alla uppgifter
            {
                Console.WriteLine($"Runda {round} av 15.");
                Task.PressEnter("börja rundan");

                for (int i = 0; i < playerArray.Length; i++) //denna loop sköter varje runda och ser till att alla spelare får köra den rundan
                {
                    if (playerArray[i].name.EndsWith("s"))
                        Console.WriteLine($"Nu är det {playerArray[i].name} tur att kasta!");
                    else
                        Console.WriteLine($"Nu är det {playerArray[i].name}s tur att kasta!");

                    bool lastTry = false;

                    for (int försök = 1; försök <= 3; försök++) //denna loop sköter den nuvarande spelarens omgång
                    {
                        if (försök == 3)
                            lastTry = true;

                        Task.PressEnter("slå tärningarna");

                        var rnd = new Random();
                        List<int> diceValues = new List<int>();
                        for (int j = 0; j < dice.Length; j++)
                        {
                            dice[j] = rnd.Next(1, 7);
                            diceValues.Add(dice[j]);

                            Console.WriteLine($"Tärning {j + 1} = {dice[j]}");
                        }
                        Console.WriteLine();

                        spelTabell = Program.CheckTasks(diceValues, spelTabell, i, lastTry);
                    }
                }
            }
        }

        private static Dictionary<User, List<GameTasks>> CheckTasks(List<int> rolledDice, Dictionary<User, List<GameTasks>> tabell, int playerPos, bool lastTry)
        {
            //tabellen ska returneras i ett av sluten av funktionen

            var temp = tabell.ToList(); //splittar dictionariet med spelarna och deras listor med gametasks så att man kan komma åt värdena
            var currentPlayerTaskArray = temp[playerPos].Value.ToArray(); //omvandlar List<Gametasks> i playersTasks(i 'game' funktionen) till en array'

            //playerPos är indexet för den nuvarande spelaren i playersTasks-dictionariet

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

            while (true)
            {
                for (int taskNum = 0; taskNum < currentPlayerTaskArray.Length; taskNum++)
                {
                    if (taskNum == 6)
                    {
                        if (currentPlayerTaskArray[0].completed == true && currentPlayerTaskArray[1].completed == true &&
                                currentPlayerTaskArray[2].completed == true && currentPlayerTaskArray[3].completed == true &&
                                currentPlayerTaskArray[4].completed == true && currentPlayerTaskArray[5].completed == true &&
                                currentPlayerTaskArray[taskNum].completed == false)
                        { //kollar om spelaren har klarat ettor, tvåor o.s.v, samt att spelaren inte redan har klarat bonusen

                            currentPlayerTaskArray[taskNum].available = false;

                            currentPlayerTaskArray[taskNum].completed = true;
                            temp[playerPos].Key.score += currentPlayerTaskArray[taskNum].score;

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(currentPlayerTaskArray[taskNum].name + "\n");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (currentPlayerTaskArray[taskNum].completed == true)
                        {
                            currentPlayerTaskArray[taskNum].available = false;

                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine(currentPlayerTaskArray[taskNum].name + "\n");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    else if (currentPlayerTaskArray[taskNum].completed == true)
                    {
                        currentPlayerTaskArray[taskNum].available = false;

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{currentPlayerTaskArray[taskNum].key} - {currentPlayerTaskArray[taskNum].name}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (currentPlayerTaskArray[taskNum].failed == true)
                    {
                        currentPlayerTaskArray[taskNum].available = false;

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{currentPlayerTaskArray[taskNum].key} - {currentPlayerTaskArray[taskNum].name}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        if (taskNum == 0) //Ettor
                        {
                            if (antalEttor >= 3)
                                currentPlayerTaskArray[taskNum].available = true;
                            else
                                currentPlayerTaskArray[taskNum].available = false;
                        }
                        else if (taskNum == 1) //Tvåor
                        {
                            if (antalTvåor >= 3)
                                currentPlayerTaskArray[taskNum].available = true;
                            else
                                currentPlayerTaskArray[taskNum].available = false;
                        }
                        else if (taskNum == 2) //Treor
                        {
                            if (antalTreor >= 3)
                                currentPlayerTaskArray[taskNum].available = true;
                            else
                                currentPlayerTaskArray[taskNum].available = false;
                        }
                        else if (taskNum == 3) //Fyror
                        {
                            if (antalFyror >= 3)
                                currentPlayerTaskArray[taskNum].available = true;
                            else
                                currentPlayerTaskArray[taskNum].available = false;
                        }
                        else if (taskNum == 4) //Femmor
                        {
                            if (antalFemmor >= 3)
                                currentPlayerTaskArray[taskNum].available = true;
                            else
                                currentPlayerTaskArray[taskNum].available = false;
                        }
                        else if (taskNum == 5) //Sexor
                        {
                            if (antalSexor >= 3)
                                currentPlayerTaskArray[taskNum].available = true;
                            else
                                currentPlayerTaskArray[taskNum].available = false;
                        }

                        //Bonus-tasken kollas innan, alltså taskNum == 6 kollas inte här

                        else if (taskNum == 7) //Par
                        {
                            if (listOfDiceNums.Contains(2) || listOfDiceNums.Contains(3) || listOfDiceNums.Contains(4) || listOfDiceNums.Contains(5))
                                currentPlayerTaskArray[taskNum].available = true;
                            else
                                currentPlayerTaskArray[taskNum].available = false;
                        }
                        else if (taskNum == 8) // Två par
                        {
                            //FIXA WALLAAAAAA!!!!!!!!!
                        }
                        else if (taskNum == 9) //Tretal
                        {
                            if (listOfDiceNums.Contains(3) || listOfDiceNums.Contains(4) || listOfDiceNums.Contains(5))
                                currentPlayerTaskArray[taskNum].available = true;
                            else
                                currentPlayerTaskArray[taskNum].available = false;
                        }
                        else if (taskNum == 10) //Fyrtal
                        {
                            if (listOfDiceNums.Contains(4) || listOfDiceNums.Contains(5))
                                currentPlayerTaskArray[taskNum].available = true;
                            else
                                currentPlayerTaskArray[taskNum].available = false;
                        }
                        else if (taskNum == 11) //Liten stege
                        {
                            if (antalEttor == 1 && antalTvåor == 1 &&
                                antalTreor == 1 && antalFyror == 1 && antalFemmor == 1)
                                currentPlayerTaskArray[taskNum].available = true;
                            else
                                currentPlayerTaskArray[taskNum].available = false;
                        }
                        else if (taskNum == 12) //Stor stege
                        {
                            if (antalTvåor == 1 && antalTreor == 1 &&
                                antalFyror == 1 && antalFemmor == 1 && antalSexor == 1)
                                currentPlayerTaskArray[taskNum].available = true;
                            else
                                currentPlayerTaskArray[taskNum].available = false;
                        }
                        else if (taskNum == 13) //Kåk
                        {
                            if (listOfDiceNums.Contains(3) && listOfDiceNums.Contains(2))
                                currentPlayerTaskArray[taskNum].available = true;
                            else
                                currentPlayerTaskArray[taskNum].available = false;
                        }
                        else if (taskNum == 14) //Chans
                        {
                            currentPlayerTaskArray[taskNum].available = true;
                        }
                        else if (taskNum == 15) //Yatzy
                        {
                            if (listOfDiceNums.Contains(5))
                                currentPlayerTaskArray[taskNum].available = true;
                            else
                                currentPlayerTaskArray[taskNum].available = false;
                        }

                        //skriv ut om den kan väljas (vit-färg) eller om den inte kan väljas (grå-färg)
                        if (currentPlayerTaskArray[taskNum].available == true)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"{currentPlayerTaskArray[taskNum].key} - {currentPlayerTaskArray[taskNum].name}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                            Console.WriteLine($"{currentPlayerTaskArray[taskNum].key} - {currentPlayerTaskArray[taskNum].name}");
                    }
                }
                bool aTaskIsAvailable = false;

                foreach (GameTasks task in currentPlayerTaskArray)
                {
                    if (task.available == true)
                        aTaskIsAvailable = true;
                }

                ConsoleKey chosenTaskKey;

                if (aTaskIsAvailable == true)
                {
                    Console.WriteLine("\nDu kan välja om du vill klara en uppgift nu eller välja tärningar och slå om.");
                    Console.WriteLine("[Enter] = klara en uppgift | [Space] = välja och slå om");

                    ConsoleKey ans = Console.ReadKey(true).Key;
                    if (ans == ConsoleKey.Enter)
                    {
                        Console.WriteLine("");
                    }
                }

                else if (lastTry == true)
                {
                    Console.WriteLine("\nDu har inte en enda kombination för att klara en uppgift.");
                    Console.WriteLine("Du måste nu stryka en uppgift.\n");

                    Console.WriteLine("\nVilken uppgift vill du stryka?");
                    Console.WriteLine("Klicka på motsvarande tangent för att välja.");

                    chosenTaskKey = Console.ReadKey(true).Key;

                    for (int i = 0; i < currentPlayerTaskArray.Length; i++)
                    {
                        if (chosenTaskKey == currentPlayerTaskArray[i].key)
                        {
                            ConsoleKey tempAns = Task.AreYouSure($"stryka uppgiften: {currentPlayerTaskArray[i].name}");
                            if (tempAns == ConsoleKey.Enter)
                            {
                                currentPlayerTaskArray[i].failed = true;
                                temp[playerPos] = new KeyValuePair<User, List<GameTasks>>(temp[playerPos].Key, currentPlayerTaskArray.ToList());
                                return tabell = temp.ToDictionary(x => x.Key, x => x.Value);
                            }
                            else
                            {
                                Console.Clear();
                                break;
                            }
                        }
                    }
                }
            }
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