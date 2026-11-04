using System;
using System.Collections.Generic;
using System.Text;
using Spectre.Console;

namespace RestaurangSystem.UI
{
    public static class MenuHelper
    {
        //Methods
        public static void MainMenu()
        {

        }
        public static void ChoiceMenu()
        {

        }
        public static void ShowMenuItems()
        {
            
        }
        public static void LogInMenu()
        {

        }
        public static void ShowOrderMenu()
        {

            bool running = true;

            while (running)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Orders - what would you like to do?[/]")
                        .AddChoices("Create new order", "Add item to order", "Show orders", "Back"));

                switch (choice)
                {
                    case "Create new order":
                        Console.WriteLine("CreateOrder() goes here");
                        break;

                    case "Add item to order":
                        Console.WriteLine("AddToOrder() goes here");
                        break;

                    case "Show orders":
                        Console.WriteLine("ShowOrders() goes here");
                        break;

                    case "Back":
                        running = false;
                        break;
                }

            }
        }

        public static void ReturnToMainMenu()
        {

        }
        public static void StaffMenu()
        {

        }
        public static void GuestMenu()
        {

        }
    }
}
