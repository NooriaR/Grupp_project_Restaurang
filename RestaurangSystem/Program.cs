using System;
using RestaurangSystem.Models;
using RestaurangSystem.Services;
using RestaurangSystem.UI;
using Spectre.Console;
namespace RestaurangSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RestaurantMenu menu = new RestaurantMenu();

            bool running = true;

            // Programmets huvudmeny
            while (running)
            {
                AnsiConsole.WriteLine("\n===== RESTAURANGSYSTEM =====");
                AnsiConsole.WriteLine("1. Lägg till maträtt");
                AnsiConsole.WriteLine("2. Ta bort maträtt");
                AnsiConsole.WriteLine("3. Avsluta");

                AnsiConsole.Write("Välj alternativ: ");
                string choice = Console.ReadLine();

                // Kontrollerar användarens val
                switch (choice)
                {
                    case "1":
                        // Anropar metoden i RestaurantMenu.cs
                        menu.AddMenuItem();
                        break;

                    case "2":
                        // Anropar metoden i RestaurantMenu.cs
                        menu.RemoveMenuItem();
                        break;  

                    case "3":
                        // Avslutar programmet
                        running = false;
                        Console.WriteLine("Programmet avslutas.");
                        break;

                    default:
                        Console.WriteLine("Ogiltigt alternativ!");
                        break;
                }
            }
            MenuHelper.ShowOrderMenu();
        }
    }    
}