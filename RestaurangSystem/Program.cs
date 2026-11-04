
using System;
using RestaurangSystem.Models;
using Spectre.Console;

﻿using RestaurangSystem.UI;

namespace RestaurangSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
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

            //AddChoices("Create new order", "Add item to order", "Show orders", "Back"));
            //case "Beställningar":
            //ShowOrderMenu();
            //break;
        }
    }
}
