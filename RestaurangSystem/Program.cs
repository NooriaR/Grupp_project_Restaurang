
using System;
using RestaurangSystem.Models;

namespace RestaurangSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Skapar ett objekt av RestaurantMenu
            RestaurantMenu menu = new RestaurantMenu();

            bool running = true;

            // Programmets huvudmeny
            while (running)
            {
                Console.WriteLine("\n===== RESTAURANGSYSTEM =====");
                Console.WriteLine("1. Lägg till maträtt");
                Console.WriteLine("2. Avsluta");

                Console.Write("Välj alternativ: ");
                string choice = Console.ReadLine();

                // Kontrollerar användarens val
                switch (choice)
                {
                    case "1":
                        // Anropar metoden i RestaurantMenu.cs
                        menu.AddMenuItem();
                        break;

                    case "2":
                        // Avslutar programmet
                        running = false;
                        Console.WriteLine("Programmet avslutas.");
                        break;

                    default:
                        Console.WriteLine("Ogiltigt alternativ!");
                        break;
                }
            }
        }
    }
}
