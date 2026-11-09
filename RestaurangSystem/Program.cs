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
            DataStore<MenuItem> dataStore = new DataStore<MenuItem>();


            // Testdata för SortByPrice, tas bort sen.
            dataStore.AddItem(new MenuItem { Name = "Pizza", Price = 120 });
            dataStore.AddItem(new MenuItem { Name = "Cola", Price = 35 });
            dataStore.AddItem(new MenuItem { Name = "Pasta", Price = 89 });

            bool running = true;

            // Programmets huvudmeny
            while (running)
            {
                AnsiConsole.WriteLine("\n===== RESTAURANGSYSTEM =====");
                AnsiConsole.WriteLine("1. Lägg till maträtt");
                AnsiConsole.WriteLine("2. Ta bort maträtt");
                AnsiConsole.WriteLine("3. Uppdatera maträtt");
                AnsiConsole.WriteLine("4. Sortera maträtterna från billigast till dyrast");
                AnsiConsole.WriteLine("5. Avsluta");

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
                   menu.RemoveMenuItem(dataStore);
                        break;

                    case "3":
                        // Anropar metoden i RestaurantMenu.cs
                        menu.UppdateMenuItem(dataStore);
                        break;

                    case "4":
                        // Anropar metoden i RestaurantMenu.cs
                        menu.SortByPrice(dataStore);
                        break;

                    case "5":
                        // Avslutar programmet
                        running = false;
                        Console.WriteLine("Programmet avslutas.");
                        break;

                    default:
                        Console.WriteLine("Ogiltigt alternativ!");
                        break;
                }
            }
 
            menu.SortByPrice(dataStore);
        }
    }    
}