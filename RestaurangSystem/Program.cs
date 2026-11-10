using System;
using RestaurangSystem.Models;
using RestaurangSystem.Services;
using RestaurangSystem.UI;

namespace RestaurangSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
<<<<<<< HEAD
=======
            RestaurantMenu menu = new RestaurantMenu();
            DataStore<MenuItem> dataStore = new DataStore<MenuItem>();


            // Testdata för SortByPrice, tas bort sen.
            dataStore.AddItem(new MenuItem { Name = "Pizza", Price = 120, Category = Category.Huvudrätt});
            dataStore.AddItem(new MenuItem { Name = "Glass", Price = 45, Category = Category.Efterrätt });

            bool running = true;

            // Programmets huvudmeny
            while (running)
            {
                AnsiConsole.WriteLine("\n===== RESTAURANGSYSTEM =====");
                AnsiConsole.WriteLine("1. Lägg till maträtt");
                AnsiConsole.WriteLine("2. Ta bort maträtt");
                AnsiConsole.WriteLine("3. Uppdatera maträtt");
                AnsiConsole.WriteLine("4. Sortera maträtterna från billigast till dyrast");
                AnsiConsole.WriteLine("5. Visa endast vegetariska rätter");
                AnsiConsole.WriteLine("6. Visa enligt kategori");
                AnsiConsole.WriteLine("7. Avsluta");

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
                        menu.ShowOnlyVegetarian(dataStore);
                        break;

                    case "6":
                        menu.ShowByCategory(dataStore);
                        break;

                    case "7":
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
>>>>>>> 85b51f040396b643644bd027f523f90ac7018112
            MenuHelper.ReturnToMainMenu();
        }
    }
}