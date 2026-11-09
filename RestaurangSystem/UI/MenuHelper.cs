using RestaurangSystem.Models;
using RestaurangSystem.Services;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurangSystem.UI
{
    public static class MenuHelper
    {
        public enum StartMenuChoice // this enum is used to simplify collecting choice from StartMenu, could set each state as a int for ease of access
        {
            Order,
            LogIn,
            Exit
        }
        //Methods
        public static StartMenuChoice StartMenu() // this method returns a enum called StartMenuChoice which could be converted to an int for easy acces in switch cases
        {
            var accent = Color.Orange1;   // change this one line to retheme everything

            AnsiConsole.Clear();
            AnsiConsole.Write(
                new Panel("Welcome! Pick an option below.")
                    .Header($"[{accent.ToMarkup()}]Restauraunt Manager[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(accent));

                return AnsiConsole.Prompt(
                     new SelectionPrompt<StartMenuChoice>()
                    .Title($"[bold {accent.ToMarkup()}]What would you like to do?[/]")
                    .PageSize(10)
                    .WrapAround()
                    .HighlightStyle(new Style(foreground: accent, decoration: Decoration.Bold))
                    .UseConverter(choice => choice switch
                    {
                        StartMenuChoice.Order => "Order",
                        StartMenuChoice.LogIn => "Log In",
                        StartMenuChoice.Exit => "Exit",
                        _ => choice.ToString()
                    })
                    .AddChoices(Enum.GetValues<StartMenuChoice>()));

        }
        public static void ChoiceMenu()
        {

        }
        public static void ShowMenuItems(DataStore<MenuItem> dataStore)
        {
            // 1. Check if there is anything to show
            if (dataStore.items.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]The menu is empty.[/]");
                return;
            }

            // 2. Create the table and its columns
            var table = new Table();
            table.AddColumn("Id");
            table.AddColumn("Name");
            table.AddColumn("Price");

            // 3. Add one row per menu item
            foreach (var item in dataStore.items)
            {
                table.AddRow(
                    item.Id.ToString(),
                    item.Name,
                    $"{item.Price:F2} kr");
            }

            // 4. Draw the table
            AnsiConsole.Write(table);

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
            StartMenuChoice choice = StartMenu();

            switch (choice)
            {
                case StartMenuChoice.Order:
                    ShowOrderMenu();
                    break;

                case StartMenuChoice.LogIn:
                    LogInMenu();
                    break;

                case StartMenuChoice.Exit:
                    Environment.Exit(0);
                    break;
            }
        }
        
        public static void StaffMenu()
        {

        }
        public static void GuestMenu()
        {

        }
    }
}
