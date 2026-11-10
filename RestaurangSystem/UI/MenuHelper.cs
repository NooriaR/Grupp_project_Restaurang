using RestaurangSystem.Models;
using RestaurangSystem.Services;
using Spectre.Console;
namespace RestaurangSystem.UI
{
    public static class MenuHelper
    {
        public static Color Accent = Color.Orange1;

        // Methods
        public static StartMenuChoice StartMenu()
        {

            AnsiConsole.Clear();
            AnsiConsole.Write(
                new Panel("Welcome! Pick an option below.")
                    .Header($"[{Accent.ToMarkup()}]Restauraunt Manager[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Accent));

            return AnsiConsole.Prompt(
                new SelectionPrompt<StartMenuChoice>()
                    .Title($"[bold {Accent.ToMarkup()}]What would you like to do?[/]")
                    .PageSize(10)
                    .WrapAround()
                    .HighlightStyle(new Style(foreground: Accent, decoration: Decoration.Bold))
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
            if (dataStore.items.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]The menu is empty.[/]");
                return;
            }

            var table = new Table();
            table.AddColumn("Id");
            table.AddColumn("Name");
            table.AddColumn("Price");

            foreach (var item in dataStore.items)
            {
                table.AddRow(
                    item.Id.ToString(),
                    item.Name,
                    $"{item.Price:F2} kr");
            }

            AnsiConsole.Write(table);
        }

        public static bool LogInMenu(DataStore<Staff> staffList) // this menu takes an datastore object with staffs and returns true if log in succesful or false if log in unsuccesfull
        {
            const int maxAttempts = 3; // a conatant int for how many attempts are allowed

            for (int attempt = 1; attempt <= maxAttempts; attempt++) // this for loop prints the menu each time until attempts reach max attempts
            {
                AnsiConsole.Clear();

                AnsiConsole.Write(
                    new Panel("Please log in to continue")
                        .Header($"[{Accent.ToMarkup()}]Login[/]")
                        .Border(BoxBorder.Rounded)
                        .BorderColor(Accent));

                var email = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold]Email[/]:")
                        .Validate(e => AuthService.IsValidEmail(e) // validates the email and continues if correct, or restart again if failed
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Enter a valid email, like name@company.com[/]")));

                var password = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold]Password[/]:").Secret()); // hides the password with *

                bool ok = AnsiConsole.Status().Start("Checking...", ctx => // this bool becomes true if login method succeeds
                {
                    Thread.Sleep(800);
                    return AuthService.LogIn(email, password,staffList);
                });

                if (ok) // this prints a message and returns true.
                {
                    AnsiConsole.MarkupLine($"[green]Welcome back {AuthService.currentStaff.Name}![/]");
                    Thread.Sleep(600);
                    return true;
                }

                AnsiConsole.MarkupLine($"[red]Wrong email or password.[/] {maxAttempts - attempt} attempt(s) left.");
                Thread.Sleep(1200);
            }

            return false; // returns false if the for loops fails
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
            bool running = true;

            while (running)
            {
                StartMenuChoice choice = StartMenu();

                switch (choice)
                {
                    case StartMenuChoice.Order:
                        ShowOrderMenu();
                        break;

                    case StartMenuChoice.LogIn:
                        //LogInMenu();
                        break;

                    case StartMenuChoice.Exit:
                        running = false;
                        break;
                }
            }
        }

        public static StaffMenuChoice StaffMenu()
        {
            var accent = Color.Blue1;

            AnsiConsole.Clear();
            AnsiConsole.Write(
                new Panel("Welcome! Pick an option below.")
                    .Header($"[{accent.ToMarkup()}]Restauraunt Manager[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(accent));

            return AnsiConsole.Prompt(
                new SelectionPrompt<StaffMenuChoice>()
                    .Title($"[bold {accent.ToMarkup()}]What would you like to do?[/]")
                    .PageSize(10)
                    .WrapAround()
                    .HighlightStyle(new Style(foreground: accent, decoration: Decoration.Bold))
                    .UseConverter(choice => choice switch
                    {
                        StaffMenuChoice.AddToMenu => "Add to Menu",
                        StaffMenuChoice.RemovefromMenu => "Remove from Menu",
                        StaffMenuChoice.UpdateitemonMenu => "Update item on Menu",
                        StaffMenuChoice.ManageOrders => "Manage Orders",
                        StaffMenuChoice.ShowSchedule => "Show Schedule",
                        StaffMenuChoice.Logout => "Log out",
                        _ => choice.ToString()
                    })
                    .AddChoices(Enum.GetValues<StaffMenuChoice>()));
        }

        public static void GuestMenu()
        {
        }
        public static void ShowError(string message, string title = "Error") // this prints a generic message which takes a string to write what you want, and title is changeble
        {
            AnsiConsole.Write(
                new Panel(new Markup($"[red]{Markup.Escape(message)}[/]"))
                    .Header($"[bold red]{title}[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Color.Red)
                    .Padding(1, 0));
        }
    }
}

