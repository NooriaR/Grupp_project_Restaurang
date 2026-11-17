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


        // Metoder
        public static StartMenuChoice StartMenu() // den här metoden visar startmenyn och returnerar det alternativ användaren väljer
        {

            AnsiConsole.Clear(); // rensar skärmen så att menyn alltid ritas på en ren skärm
            AnsiConsole.Write(
                new Panel("Welcome! Pick an option below.") // rutan med välkomsttexten
                    .Header($"[{Accent.ToMarkup()}]Restauraunt Manager[/]") // rubriken i rutans övre kant, i accentfärgen
                    .Border(BoxBorder.Rounded) // rundade hörn på ramen
                    .BorderColor(Accent)); // ramen får samma accentfärg som resten av menyn

            return AnsiConsole.Prompt( // visar valmenyn, väntar på valet och returnerar det direkt
                new SelectionPrompt<StartMenuChoice>() // valen är enum-värden, så resultatet är en StartMenuChoice och inte en text
                    .Title($"[bold {Accent.ToMarkup()}]What would you like to do?[/]") // frågan som visas ovanför valen
                    .PageSize(10) // hur många rader menyn visar åt gången
                    .WrapAround() // går man förbi sista valet hamnar man på det första, och tvärtom
                    .HighlightStyle(new Style(foreground: Accent, decoration: Decoration.Bold)) // det markerade valet blir fetstilt i accentfärgen
                    .UseConverter(choice => choice switch // bestämmer vilken text användaren ser för varje enum-värde
                    {
                        StartMenuChoice.GuestView => "Guest View", // texten som visas för GuestView
                        StartMenuChoice.StaffView => "Staff View", // texten som visas för StaffView
                        StartMenuChoice.Exit => "Exit", // texten som visas för Exit
                        _ => choice.ToString() // reservval: visar enum-namnet om något värde saknas ovan
                    })
                    .AddChoices(Enum.GetValues<StartMenuChoice>())); // lägger in alla värden i enumen som val, så nya värden dyker upp automatiskt
        }

        public static string StaffPortal() // den här metoden visar personalportalens meny och returnerar texten på det val användaren gör
        {
            return AnsiConsole.Prompt( // visar menyn, väntar på valet och returnerar det direkt
            new SelectionPrompt<string>() // valen är vanliga strängar, så resultatet är en text och inte ett enum-värde
                .Title("[green]What do you want to do?[/]") // frågan som visas ovanför valen, i grönt
                .AddChoices("Log In", "Register", "Back")); // de tre valen som användaren kan välja mellan
        }
        // Visar alla maträtter och deras information i en tabell.
        public static void ShowMenuItems(DataStore<MenuItem> dataStore)
        {
            // Visar alla maträtter och deras information i en tabell.
            if (dataStore.items.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]The menu is empty.[/]");
                return;  // Avslutar metoden om det inte finns några maträtter.
            }
            // Skapar en ny tabell för att visa maträtternas information.
            var table = new Table();
            // Lägger till kolumner för maträttens ID, namn och pris.
            table.AddColumn("Id");
            table.AddColumn("Name");
            table.AddColumn("Price");
            // Går igenom alla maträtter i listan.
            foreach (var item in dataStore.items)
            {
                // Lägger till maträttens information som en ny rad i tabellen.
                table.AddRow(
                    item.Id.ToString(),
                    item.Name,
                    $"{item.Price:F2} kr"); // Visar priset med två decimaler
            }
            // Skriver ut tabellen på skärmen.
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
        // Visar ordermenyn och returnerar det alternativ som användaren väljer.
        public static string ShowOrderMenu()
        {
            return AnsiConsole.Prompt(
                     // Skapar en meny där användaren kan välja ett alternativ.
                     new SelectionPrompt<string>()
                         .Title("[green]Orders - what would you like to do?[/]")
                         // Lägger till alternativen för att visa ordrar, uppdatera orderstatus eller gå tillbaka.
                         .AddChoices("Show orders", "Update Order Status", "Back"));
        }

        public static StaffMenuChoice StaffMenu() // den här metoden visar personalmenyn och returnerar det alternativ användaren väljer
        {
            var accent = Color.Blue1; // en lokal accentfärg för den här menyn, så att personalmenyn får ett eget utseende

            AnsiConsole.Clear(); // rensar skärmen så att menyn alltid ritas på en ren skärm
            AnsiConsole.Write(
                new Panel($"Welcome {AuthService.currentStaff.Name}! Pick an option below.") // rutan med välkomsttexten
                    .Header($"[{accent.ToMarkup()}]Restauraunt Manager[/]") // rubriken i rutans övre kant, i accentfärgen
                    .Border(BoxBorder.Rounded) // rundade hörn på ramen
                    .BorderColor(accent)); // ramen får samma färg som resten av menyn

            return AnsiConsole.Prompt( // visar valmenyn, väntar på valet och returnerar det direkt
                new SelectionPrompt<StaffMenuChoice>() // valen är enum-värden, så resultatet är en StaffMenuChoice och inte en text
                    .Title($"[bold {accent.ToMarkup()}]What would you like to do?[/]") // frågan som visas ovanför valen
                    .PageSize(10) // hur många rader menyn visar åt gången
                    .WrapAround() // går man förbi sista valet hamnar man på det första, och tvärtom
                    .HighlightStyle(new Style(foreground: accent, decoration: Decoration.Bold)) // det markerade valet blir fetstilt i accentfärgen
                    .UseConverter(choice => choice switch // bestämmer vilken text användaren ser för varje enum-värde
                    {
                        StaffMenuChoice.AddToMenu => "Add to Menu", // texten som visas för AddToMenu
                        StaffMenuChoice.RemovefromMenu => "Remove from Menu", // texten som visas för RemovefromMenu
                        StaffMenuChoice.UpdateitemonMenu => "Update item on Menu", // texten som visas för UpdateitemonMenu
                        StaffMenuChoice.ManageOrders => "Manage Orders", // texten som visas för ManageOrders
                        StaffMenuChoice.ShowSchedule => "Show Schedule", // texten som visas för ShowSchedule
                        StaffMenuChoice.Logout => "Log out", // texten som visas för Logout
                        _ => choice.ToString() // reservval: visar enum-namnet om något värde saknas ovan
                    })
                    .AddChoices(Enum.GetValues<StaffMenuChoice>())); // lägger in alla värden i enumen som val, så nya värden dyker upp automatiskt
        }

        public static GuestMenuChoice GuestMenu(Guest guest) // den här metoden visar gästmenyn och returnerar det alternativ gästen väljer
        {

            // Visa menyn och spara gästens val
            AnsiConsole.Clear(); // rensar skärmen så att menyn alltid ritas på en ren skärm
            AnsiConsole.Write(
                new Panel($"Welcome,{guest.Name}! Pick an option below.") // rutan med välkomsttexten
                    .Header($"[{Accent.ToMarkup()}]Restauraunt Manager[/]") // rubriken i rutans övre kant, i accentfärgen
                    .Border(BoxBorder.Rounded) // rundade hörn på ramen
                    .BorderColor(Accent)); // ramen får samma accentfärg som resten av menyn

            return AnsiConsole.Prompt( // visar valmenyn, väntar på valet och returnerar det direkt
                new SelectionPrompt<GuestMenuChoice>() // valen är enum-värden, så resultatet är en GuestMenuChoice och inte en text
                    .Title($"[bold {Accent.ToMarkup()}]What would you like to do?[/]") // frågan som visas ovanför valen
                    .PageSize(10) // hur många rader menyn visar åt gången
                    .WrapAround() // går man förbi sista valet hamnar man på det första, och tvärtom
                    .HighlightStyle(new Style(foreground: Accent, decoration: Decoration.Bold)) // det markerade valet blir fetstilt i accentfärgen
                    .UseConverter(choice => choice switch // bestämmer vilken text användaren ser för varje enum-värde
                    {
                        GuestMenuChoice.CreateOrder => "Create Order", // texten som visas för CreateOrder
                        GuestMenuChoice.ShowMenu => "Show Menu", // texten som visas för ShowMenu
                        GuestMenuChoice.ShowOrderStatus => "Show Order Status", // texten som visas för ShowOrderStatus
                        GuestMenuChoice.MakeReservation => "Make Reservation", // texten som visas för MakeReservation
                        GuestMenuChoice.ShowReservation => "Show Reservation", // texten som visas för ShowReservation
                        GuestMenuChoice.CancelReservation => "Cancel Reservation", // texten som visas för CancelReservation
                        GuestMenuChoice.Back => "Back", // texten som visas för Back
                        _ => choice.ToString() // reservval: visar enum-namnet om något värde saknas ovan
                    })
                    .AddChoices(Enum.GetValues<GuestMenuChoice>())); // lägger in alla värden i enumen som val, så nya värden dyker upp automatiskt
        }
        // Visar menyn för att återställa personalens lösenord.
        public static void ResetPasswordMenu(DataStore<Staff> staffList)
        {
            // Rensar konsolen innan återställningsmenyn visas.
            AnsiConsole.Clear();
            // Visar en ruta med information om hur lösenordet återställs.
            AnsiConsole.Write(
                new Panel("Enter your email to reset your password")
                    .Header($"[{Accent.ToMarkup()}]Reset password[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Accent));
            // Ber användaren att ange sin e-postadress och kontrollerar att formatet är giltigt.
            var email = AnsiConsole.Prompt(
                new TextPrompt<string>("[bold]Email[/]:")
                    .Validate(e => AuthService.IsValidEmail(e)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("Enter a valid email")));
            // Försöker starta lösenordsåterställningen för det angivna e-postkontot.
            if (!AuthService.StartReset(email, staffList, out var code))
            {
                // Visar ett felmeddelande om inget konto hittas med den angivna e-postadressen.
                ShowError("No account with that email.");
                // Väntar en kort stund så att användaren hinner läsa felmeddelandet.
                Thread.Sleep(1500);
                // Avslutar metoden om återställningen inte kunde startas.
                return;
            }

            // Ingen riktig e-post skickas, så koden visas här istället
            AnsiConsole.Write(
                new Panel($"Your reset code is [bold]{code}[/]")
                    .Header("Simulated email")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Color.Grey));
            // Ber användaren att ange återställningskoden.
            var enteredCode = AnsiConsole.Prompt(new TextPrompt<string>("[bold]Reset code[/]:"));
            // Ber användaren att ange ett nytt lösenord och kontrollerar att det är minst 6 tecken långt.
            var newPassword = AnsiConsole.Prompt(
                new TextPrompt<string>("[bold]New password[/]:")
                    .Secret()
                    .Validate(p => p.Length >= 6
                        ? ValidationResult.Success()
                        : ValidationResult.Error("At least 6 characters")));
            // Ber användaren att bekräfta det nya lösenordet och kontrollerar att det stämmer.
            AnsiConsole.Prompt(
                new TextPrompt<string>("[bold]Confirm password[/]:")
                    .Secret()
                    .Validate(p => p == newPassword
                        ? ValidationResult.Success()
                        : ValidationResult.Error(("Passwords don't match"))));
            // Kontrollerar återställningskoden och försöker spara det nya lösenordet.
            if (AuthService.CompleteReset(enteredCode, newPassword, staffList))
            {
                // Loggar att lösenordet har återställts.
                Logger.Info($"Password reset for {email}");
                // Meddelar användaren att lösenordet har uppdaterats.
                AnsiConsole.MarkupLine("[green]Password updated. You can log in now.[/]");
            }
            else
            {
                // Loggar att lösenordsåterställningen misslyckades.
                Logger.Warning($"Failed password reset for {email}");
                // Visar ett felmeddelande om återställningskoden är felaktig. 
                ShowError("Wrong reset code.");
            }
            // Väntar en kort stund så att användaren hinner läsa meddelandet.
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

