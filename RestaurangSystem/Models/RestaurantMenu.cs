using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Spectre.Console;
﻿using Spectre.Console;

namespace RestaurangSystem.Models
{
    public class RestaurantMenu
    {
        //Attributes
        public List<MenuItem> MenuItems = new List<MenuItem>();
        //Methods
        public void AddMenuItem()
        {
            // Hämtar maträttens namn
            AnsiConsole.Write("Ange maträttens namn: ");
            string namn = Console.ReadLine();

            // Hämtar priset och kontrollerar att det är giltigt
            AnsiConsole.Write("Ange pris: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal pris) || pris < 0)
            {
                AnsiConsole.WriteLine("Ogiltigt pris!");
                return;
            }

            // Hämtar maträttens innehåll
            AnsiConsole.Write("Ange innehåll: ");
            string innehall = Console.ReadLine();

            // Frågar om maträtten är vegetarisk
            AnsiConsole.Write("Är maträtten vegetarisk? (ja/nej): ");
            string svar = Console.ReadLine().ToLower();

            bool isVegeterian;

            if (svar == "ja")
            {
                isVegeterian = true;
            }
            else if (svar == "nej")
            {
                isVegeterian = false;
            }
            else
            {
                Console.WriteLine("Svara ja eller nej!");
                return;
            }

            // Hämtar maträttens kategori
            AnsiConsole.Write("Ange kategori: ");
            string category = Console.ReadLine();

            // Skapar ett nytt maträttsobjekt
            MenuItem newItem = new MenuItem
            {
                Namn = namn,
                Pris = pris,
                innehåll = innehall,
                isVegeterian = isVegeterian
            };

            // Lägger till maträtten i listan
            MenuItems.Add(newItem);

            AnsiConsole.WriteLine($"{namn} har lagts till i menyn!");
        }
        public void RemoveMenuItem()
        {

        }
        public void SortByPrice()
        {

        }

        public static void ShowMenu()
        {
            bool running = true;

            while (running)
            {
                var choice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[green]What do you want to do?[/]")
                            .AddChoices("Add MenuItem", "Show Menu", "Finish"));

                switch (choice)
                {
                    case "Add MenuItem":
                        Console.WriteLine("This is the AddMenuItem()");
                        break;

                    case "Show Menu":
                        Console.WriteLine("This is the ShowMenuItems()");
                        break;

                    case "Finish":
                        running = false;
                        break;
                }
            }
        }


        public void ShowOnlyVegetarian()
        {

        }
        public void ShowByCategory()
        {

        }

    }
}
