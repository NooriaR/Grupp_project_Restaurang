using RestaurangSystem.Models;
using RestaurangSystem.Services;
using Spectre.Console;
namespace RestaurangSystem.UI
{
    public static class MenuHelper
    {
        public static Color Accent = Color.LightSkyBlue1;

        public static void ShowLogo()
        {
    // Visar restaurangens namn som stor ASCII-text
    AnsiConsole.Write(
        new FigletText("Haven")
            .Centered()
            .Color(Accent));

    // Visar en kort slogan under loggan
    AnsiConsole.Write(
        new Rule("[grey]A calm place to eat[/]")
            .RuleStyle(new Style(Accent)));
        }


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

        public static bool LogInMenu(DataStore<Staff> staffList) // den här menyn tar ett datastore-objekt med personal och returnerar true om inloggningen lyckas eller false om den misslyckas
        {
            const int maxAttempts = 3; // en konstant int för hur många försök som är tillåtna

            for (int attempt = 1; attempt <= maxAttempts; attempt++) // den här for loopen skriver ut menyn varje gång tills försöken når max antal försök
            {
                AnsiConsole.Clear();

                AnsiConsole.Write(
                    new Panel("Please log in to continue")
                        .Header($"[{Accent.ToMarkup()}]Login[/]")
                        .Border(BoxBorder.Rounded)
                        .BorderColor(Accent));

                var email = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold]Email[/]:")
                        .Validate(e => AuthService.IsValidEmail(e) // validerar epostadressen och går vidare om den är rätt, eller frågar igen om den är fel
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Enter a valid email, like name@company.com[/]")));

                var password = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold]Password[/]:").Secret()); // döljer lösenordet med *

                AnsiConsole.Progress()
                    .AutoClear(true) // tar bort stapeln när den är klar
                    .Columns(
                        new TaskDescriptionColumn(),
                        new ProgressBarColumn()
                            .CompletedStyle(new Style(Color.Green))
                            .FinishedStyle(new Style(Color.Green)),
                        new PercentageColumn())
                    .Start(ctx =>
                    {
                        var task = ctx.AddTask("Checking...");

                        while (!ctx.IsFinished) // fyller stapeln steg för steg
                        {
                            task.Increment(10);
                            Thread.Sleep(160);
                        }
                    });

                bool ok = AuthService.LogIn(email, password, staffList); // kontrollerar inloggningen när stapeln är klar

                if (ok) // om inloggningen lyckades skrivs ett meddelande ut och metoden returnerar true
                {
                    AnsiConsole.MarkupLine($"[green]Welcome back {AuthService.currentStaff.Name}![/]");
                    Thread.Sleep(600);
                    return true;
                }

                ShowError($"Wrong email or password. {maxAttempts - attempt} attempt(s) left."); // visas bara när inloggningen misslyckades
                Thread.Sleep(1200); // paus så att man hinner läsa felet innan skärmen rensas
            }

            return false; // returnerar false om for loopen misslyckas
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

        public static void GuestMenu(DataStore<MenuItem> menuStore)
        {
            // Controls whether the menu keeps showing
            bool running = true;

            while (running)
            {
                // Clear the screen so the menu is clean each time
                AnsiConsole.Clear();
                // Show the menu and store the guest's choice
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Guest - what would you like to do?[/]")
                        .AddChoices(
                            "Show menu",
                            "Show my reservation",
                            "Cancel reservation",
                            "Show order status",
                            "Back"));
                // Perform the action the guest picked
                switch (choice)
                {
                    // Show all menu items in a table
                    case "Show menu":
                        ShowMenuItems(menuStore);
                        break;

                    case "Show my reservation":
                        // Placeholder until the Guest class is ready
                        Console.WriteLine("ShowReservation() goes here");
                        break;

                    case "Cancel reservation":
                        // Placeholder until the Guest class is ready
                        Console.WriteLine("CancelReservation() goes here");
                        break;

                    case "Show order status":
                        // Placeholder until the Guest class is ready
                        Console.WriteLine("ShowOrderStatus() goes here");
                        break;

                    case "Back":
                        // End the loop and return to the previous menu
                        running = false;
                        break;
                }
                // Wait for a key press so the output can be read before the screen clears
                if (running)
                {
                    AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
                    Console.ReadKey(true);
                }
            }
        }
        public static void ShowError(string message, string title = "Error") // den här skriver ut ett generellt meddelande som tar en sträng för att skriva vad du vill, och titeln går att ändra
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

