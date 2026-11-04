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
            // ===== TILLFÄLLIGT TEST (ta bort efteråt) =====
            DataStore<MenuItem> store = new DataStore<MenuItem>();
            MenuItem item = new MenuItem();

            Console.WriteLine(store.items.Count); 
            store.AddItem(item);
            Console.WriteLine(store.items.Count); 


            Console.WriteLine(item.Id); // ska skriva 1

            // ===== SLUT PÅ TEST =====

            // MenuHelper.StartMenu(); This is how you call the Start Menu
            // Skapar ett objekt av RestaurantMenu
            RestaurantMenu menu = new RestaurantMenu();

            bool running = true;

            // Programmets huvudmeny
            while (running)
            {
                AnsiConsole.WriteLine("\n===== RESTAURANGSYSTEM =====");
                AnsiConsole.WriteLine("1. Lägg till maträtt");
                AnsiConsole.WriteLine("2. Avsluta");

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

    // Tillfällig testklass (ta bort efteråt)
    
}