using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FashionGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Fashion Game!");

            if (!File.Exists("fashion.txt"))
            {
                Console.WriteLine("Error: fashion.txt file not found!");
                return; 
            }
            string playerName = PlayerName();
            
            string password = CreatePassword();

            if (!ConfirmPassword(password))
            {
                Console.WriteLine("Login failed. Incorrect password. Exiting...");
                return;
            }

            Console.WriteLine($"Welcome, {playerName}, to the Fashion Game!");
            ConsoleSettings();

            // From line 43 to 59: - The code being dervied from price management concept from module 17 dictinories.
            int gameOption;
            do
            {
                Console.WriteLine("Choose game mode:\n1. Single Player\n2. Multi Player\n3. Exit");
                if (!int.TryParse(Console.ReadLine(), out gameOption))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (gameOption)
                {
                    case 1:
                        SinglePlayerGame(playerName);
                        break;
                    case 2:
                        MultiPlayerGame();
                        break;
                    case 3:
                        Console.WriteLine("Thanks for playing!");
                        return; 
                    default:
                        Console.WriteLine("Invalid option. Please choose again.");
                        break;
                }
                Console.WriteLine("Would you like to play again? (yes/no)");
            } while (Console.ReadLine().ToLower() == "yes");
            Console.WriteLine("Thanks for playing!");
        }
        static string PlayerName()
        {
            Console.Write("Enter your name: ");
            return Console.ReadLine();
        }

        static string CreatePassword()
        {
            Console.Write("Create a password to access the fashion world: ");
            return Console.ReadLine();
        }

        static bool ConfirmPassword(string enteredPassword)
        {
            const int maxAttempts = 3; 
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                Console.Write("Retype your password: ");
                string reenteredPassword = Console.ReadLine();

                if (enteredPassword == reenteredPassword)
                {
                    return true;
                }
                else
                {
                    attempts++;
                    Console.WriteLine($"Incorrect password. {maxAttempts - attempts} attempts left.");
                }
            }

            return false;
        }
        static void SinglePlayerGame(string playerName)
        {
            Player player = new Player(playerName);
            PlayerDetails(player, "input.txt", "Player ");
            TotalPoints(player);
            SinglePlayerResult(player);
        }

        static void MultiPlayerGame()
        {
            string player1Name = MultiPlayerName("Player 1");
            string player2Name = MultiPlayerName("Player 2");

            Player player1 = new Player(player1Name);
            Player player2 = new Player(player2Name);

            PlayerDetails(player1, "input.txt", "Player 1");
            TotalPoints(player1);

            PlayerDetails(player2, "input.txt", "Player 2");
            TotalPoints(player2);

            MultiPlayerResult(player1, player2); 
        }
        static string MultiPlayerName(string playerIdentifier)
        {
            Console.Write($"Enter {playerIdentifier}'s name: ");
            return Console.ReadLine();
        }

        //From line 140 - 162 the idea for case being taken from price management concept which is in dictionaries module 17.
        //In line 134 the idea for file being taken from Configuration file concept which is in configuration file module 19.
        // From line 151 to line 161 the part concept is taken fromStack Overflow. (n.d.). parsing string in c# and putting them into variables. [online] Available at: www.stackoverflow.com/questions/57737679/parsing-string-in-c-sharp-and-putting-them-into-variables [Accessed 7 Jan. 2024].‌
        static void ConsoleSettings()
        {
            string[] lines = File.ReadAllLines("fashion.txt");
            foreach (var line in lines)
            {
                string[] parts = line.Split('|');
                switch (parts[0])
                {
                    case "Model":
                        Player.Models.Add(parts[1]);
                        break;
                    case "Style":
                        Player.Styles.Add(parts[1]);
                        break;
                    case "Dress":
                    case "Top":
                    case "Bottom":
                    case "Coat":
                    case "Footwear":
                        if (!Player.ClothingOptions.ContainsKey(parts[0]))
                        {
                            Player.ClothingOptions[parts[0]] = new Dictionary<string, int>();
                        }
                        Player.ClothingOptions[parts[0]].Add(parts[1], int.Parse(parts[2]));
                        break;
                    case "Accessory":
                        Player.Accessories.Add(parts[1], int.Parse(parts[2]));
                        break;
                    default:
                        Console.WriteLine($"Unknown configuration type: {parts[0]}");
                        break;
                }
            }
        }
        //In line 178 the concept that is derived is being modified from Stack Overflow. (n.d.). ToList() - does it create a new list? [online] Available at: www.stackoverflow.com/questions/2774099/tolist-does-it-create-a-new-list [Accessed 5 Jan. 2024].‌
        static void PlayerDetails(Player player, string fileName, string playerName)
        {
            Console.WriteLine($"\n{playerName}, please make your selections:");

            player.SelectedModel = ChooseOption("Select a Model:", Player.Models);
            player.SelectedStyle = ChooseOption("Select a Style:", Player.Styles);

            string initialClothingType = ChooseOption("Select a type of clothing: 'Dress', 'Top', 'Bottom'", new List<string> { "Dress", "Top", "Bottom" });

            if (initialClothingType == "Dress")
            {
                List<string> dressOptions = Player.ClothingOptions[initialClothingType].Keys.ToList();
                player.SelectedDress = ChooseOption($"Select a {initialClothingType}:", dressOptions);
                player.TotalPoints += Player.ClothingOptions["Dress"][player.SelectedDress];
            }
            else if (initialClothingType == "Top")
            {
                List<string> topOptions = Player.ClothingOptions[initialClothingType].Keys.ToList();
                player.SelectedTop = ChooseOption($"Select a {initialClothingType}:", topOptions);
                player.TotalPoints += Player.ClothingOptions["Top"][player.SelectedTop];

                List<string> bottomOptions = Player.ClothingOptions["Bottom"].Keys.ToList();
                player.SelectedBottom = ChooseOption("Select a Bottom:", bottomOptions);
                player.TotalPoints += Player.ClothingOptions["Bottom"][player.SelectedBottom];
            }
            else if (initialClothingType == "Bottom")
            {
                List<string> topOptions = Player.ClothingOptions["Top"].Keys.ToList();
                player.SelectedClothing = ChooseOption("Select a Top:", topOptions);
            }
            else
            {
                Console.WriteLine("Invalid choice of clothing type.");
                return;
            }

            List<string> coatOptions = Player.ClothingOptions["Coat"].Keys.ToList();
            player.SelectedCoat = ChooseOption("Select a Coat:", coatOptions);
            player.TotalPoints += Player.ClothingOptions["Coat"][player.SelectedCoat];

            List<string> footwearOptions = Player.ClothingOptions["Footwear"].Keys.ToList();
            player.SelectedFootwear = ChooseOption("Select a Footwear:", footwearOptions);
            player.TotalPoints += Player.ClothingOptions["Footwear"][player.SelectedFootwear];

            for (int i = 0; i < 2; i++)
            {
                string accessory = ChooseOption($"Select Accessory {i + 1}:", Player.Accessories.Keys.ToList());
                player.TotalPoints += Player.Accessories[accessory];
            }
        }

        //from line 221 to 225 the concept of foreach being taken from BillWagner (n.d.). Iteration statements - C# reference. [online] learn.microsoft.com. Available at: www.learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/iteration-statements.

        static string ChooseOption(string message, List<string> options)
        {
            Console.WriteLine(message);
            int index = 1;
            foreach (var option in options)
            {
                Console.WriteLine($"{index}. {option}");
                index++;
            }

            int choice;
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out choice) && choice > 0 && choice <= options.Count)
                {
                    string selectedOption = options[choice - 1];

                    if (selectedOption.StartsWith("Dress") && (Player.ClothingOptions.ContainsKey("Top") || Player.ClothingOptions.ContainsKey("Bottom")))
                    {
                        return selectedOption;
                    }

                    if ((selectedOption.StartsWith("Top") || selectedOption.StartsWith("Bottom")) && Player.ClothingOptions.ContainsKey("Dress"))
                    {
                        return selectedOption;
                    }

                    if (selectedOption.StartsWith("Dress") || selectedOption.StartsWith("Top") || selectedOption.StartsWith("Bottom"))
                    {
                        return selectedOption;
                    }

                    return ChooseSpecificOption(selectedOption);
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid option number.");
                }
            }
        }

        static string ChooseSpecificOption(string clothingType)
        {
            switch (clothingType)
            {
                case "Coat":
                    return ChooseOption("Select a Coat:", Player.ClothingOptions["Coat"].Keys.ToList());
                case "Footwear":
                    return ChooseOption("Select a Footwear:", Player.ClothingOptions["Footwear"].Keys.ToList());
                default:
                    return clothingType;
            }
        }

        static void TotalPoints(Player player)
        {
            Console.WriteLine($"\nTotal Points for {player.PlayerName}: {player.TotalPoints}");
        }

        static void SinglePlayerResult(Player player)
        {
            if (player.TotalPoints > 300)
            {
                Console.WriteLine($"\nCongratulations, {player.PlayerName}! You've scored more than 300 points. You win!");
            }
            else
            {
                Console.WriteLine($"\nSorry, {player.PlayerName}. You did not score more than 250 points. Better luck next time!");
            } 
        }

        static void MultiPlayerResult(Player player1, Player player2)
        {
            Console.WriteLine($"\n{player1.PlayerName}'s score: {player1.TotalPoints}");
            Console.WriteLine($"{player2.PlayerName}'s score: {player2.TotalPoints}");

            if (player1.TotalPoints > player2.TotalPoints)
            {
                Console.WriteLine($"\n{player1.PlayerName} wins with {player1.TotalPoints} points!");
            }
            else if (player2.TotalPoints > player1.TotalPoints)
            {
                Console.WriteLine($"\n{player2.PlayerName} wins with {player2.TotalPoints} points!");
            }
            else
            {
                Console.WriteLine("\nIt's a tie!");
            }
        }

    }

    // From line 313 to 332 the concept is taken from BillWagner (n.d.). Classes. [online] learn.microsoft.com. Available at: www.learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/classes.
    public class Player
    {
        public static List<string> Models = new List<string>();
        public static List<string> Styles = new List<string>();
        public static Dictionary<string, Dictionary<string, int>> ClothingOptions = new Dictionary<string, Dictionary<string, int>>();
        public static Dictionary<string, int> Accessories = new Dictionary<string, int>();

        public string PlayerName { get; set; } = "Single Player";
        public string SelectedModel { get; set; }
        public string SelectedStyle { get; set; }
        public string SelectedClothing { get; set; }
        public int TotalPoints { get; set; }
        public string SelectedCoat { get; set; }
        public string SelectedFootwear { get; set; }
        public string SelectedBottom { get; set; }
        public string SelectedTop { get; set; }
        public string SelectedDress { get; set; }

        public Player(string playerName) 
        {
            PlayerName = playerName;
            TotalPoints = 0;
        }
    }
}

