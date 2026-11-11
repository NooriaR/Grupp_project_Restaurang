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
                        StartMenuChoice.GuestView => "Guest View",
                        StartMenuChoice.StaffView => "Staff View",
                        StartMenuChoice.Exit => "Exit",
                        _ => choice.ToString()
                    })
                    .AddChoices(Enum.GetValues<StartMenuChoice>()));
        }

        public static string StaffPortal()
        {
            return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]What do you want to do?[/]")
                .AddChoices("Log In", "Register", "Back"));
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
                var next = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("What do you want to do?")
                        .HighlightStyle(new Style(foreground: Accent, decoration: Decoration.Bold))
                        .AddChoices("Try again", "Reset password", "Exit"));

                if (next == "Reset password")
                {
                    ResetPasswordMenu(staffList);
                    attempt--; // återställningen ska inte räknas som ett misslyckat försök
                }
                else if (next == "Exit")
                {
                    return false;
                }
            }

            return false; // returnerar false om for loopen misslyckas
        }

        public static void ShowOrderMenu()
        {
            DataStore<Order> orderDataStore = new DataStore<Order>();
            orderDataStore.LoadFromJson();
            bool running = true;

            while (running)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Orders - what would you like to do?[/]")
                        .AddChoices("Show orders", "Update Order Status", "Back"));

                switch (choice)
                {
                    case "Show orders":
                        AuthService.currentStaff.ShowOrders();
                        break;
                    case "Update Order Status":
                        var order = orderDataStore.FindItem(o => o.Id == AnsiConsole.Ask<int>("Enter the order ID to update:"));
                        if (order != null)
                        {
                            var newStatus = AnsiConsole.Prompt(
                                new SelectionPrompt<OrderStatus>()
                                    .Title("Select the new status:")
                                    .AddChoices(Enum.GetValues<OrderStatus>()));
                            order.UpdateStatus(newStatus);
                            orderDataStore.SaveToJson();
                        }
                        break;
                    case "Back":
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

        public static GuestMenuChoice GuestMenu()
        {

            // Show the menu and store the guest's choice
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new Panel("Welcome! Pick an option below.")
                    .Header($"[{Accent.ToMarkup()}]Restauraunt Manager[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Accent));

            return AnsiConsole.Prompt(
                new SelectionPrompt<GuestMenuChoice>()
                    .Title($"[bold {Accent.ToMarkup()}]What would you like to do?[/]")
                    .PageSize(10)
                    .WrapAround()
                    .HighlightStyle(new Style(foreground: Accent, decoration: Decoration.Bold))
                    .UseConverter(choice => choice switch
                    {
                        GuestMenuChoice.CreateOrder => "Create Order",
                        GuestMenuChoice.ShowMenu => "Show Menu",
                        GuestMenuChoice.ShowOrderStatus => "Show Order Status",
                        GuestMenuChoice.Back => "Back"
                    })
                    .AddChoices(Enum.GetValues<GuestMenuChoice>()));
        }
        public static void ResetPasswordMenu(DataStore<Staff> staffList)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new Panel("Enter your email to reset your password")
                    .Header($"[{Accent.ToMarkup()}]Reset password[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Accent));

            var email = AnsiConsole.Prompt(
                new TextPrompt<string>("[bold]Email[/]:")
                    .Validate(e => AuthService.IsValidEmail(e)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("Enter a valid email")));

            if (!AuthService.StartReset(email, staffList, out var code))
            {
                ShowError("No account with that email.");
                Thread.Sleep(1500);
                return;
            }

            // Ingen riktig e-post skickas, så koden visas här istället
            AnsiConsole.Write(
                new Panel($"Your reset code is [bold]{code}[/]")
                    .Header("Simulated email")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Color.Grey));

            var enteredCode = AnsiConsole.Prompt(new TextPrompt<string>("[bold]Reset code[/]:"));

            var newPassword = AnsiConsole.Prompt(
                new TextPrompt<string>("[bold]New password[/]:")
                    .Secret()
                    .Validate(p => p.Length >= 6
                        ? ValidationResult.Success()
                        : ValidationResult.Error("At least 6 characters")));

            AnsiConsole.Prompt(
                new TextPrompt<string>("[bold]Confirm password[/]:")
                    .Secret()
                    .Validate(p => p == newPassword
                        ? ValidationResult.Success()
                        : ValidationResult.Error(("Passwords don't match"))));

            if (AuthService.CompleteReset(enteredCode, newPassword, staffList))
            {
               // Logger.Info($"Password reset for {email}");
                AnsiConsole.MarkupLine("[green]Password updated. You can log in now.[/]");
            }
            else
            {
                //Logger.Warning($"Failed password reset for {email}");
                ShowError("Wrong reset code.");
            }

            Thread.Sleep(1500);
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

