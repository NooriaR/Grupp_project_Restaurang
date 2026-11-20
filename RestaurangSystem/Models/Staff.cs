﻿using RestaurangSystem.Services;
using RestaurangSystem.UI;
using Spectre.Console;
using System.Text.Json.Serialization;
namespace RestaurangSystem.Models
{
    public class Staff : Users
    {
        // Egenskaper (Attributes)

        // Lagrar personalens lösenord.
        public string Password { get; set; }

        [JsonIgnore]
        public bool IsLoggedIn { get; set; }

        // Kopplar ett arbetsschema till den anställda.
        // Varje anställd får ett eget tomt arbetsschema.
        public WorkSchedule Schedule { get; set; } = new WorkSchedule();


        // Visar alla sparade beställningar i en tabell i terminalen
        public void ShowOrders()
        {
            // Skapar ett nytt datalager för beställningar och läser in sparad data från JSON-filen
            DataStore<Order> orderDataStore = new DataStore<Order>();
            orderDataStore.LoadFromJson();

            // Hämtar listan med alla beställningar
            var orders = orderDataStore.items;

            // Om det inte finns några beställningar visas ett meddelande och metoden avslutas
            if (orders.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No orders found.[/]");
                return;
            }

            // Skapar tabellen och lägger till en kolumnrubrik för varje sorts information
            var table = new Table();
            table.AddColumn("Order ID");
            table.AddColumn("Guest Name");
            table.AddColumn("Items");
            table.AddColumn("Total Price");
            table.AddColumn("Status");

            // Går igenom varje beställning och lägger till en rad i tabellen
            foreach (var order in orders)
            {
                // Slår ihop namnen på alla rätter till en text, separerade med komma
                string items = string.Join(", ", order.OrderedItems.Select(i => i.Name));

                // Slår ihop gästernas namn till en text, ett namn per rad
                // Markup.Escape skyddar mot tecken som kan förstöra färgkoderna
                string guests = string.Join("\n", order.Guests.Select(g => Markup.Escape(g.Name)));

                // Lägger till raden. Värdena måste komma i samma ordning som kolumnerna
                table.AddRow(
                    order.Id.ToString(),
                    guests,
                    items,
                    $"{order.CalculateSumPrice()} kr",
                    order.Status.ToString());
            }

            // Skriver ut den färdiga tabellen
            AnsiConsole.Write(table);
        }

        public void Register()
        {
            // STEG 1: Skapa en DataStore för Staff och ladda det som redan är sparat
            var stafflist = new DataStore<Staff>();
            stafflist.LoadFromJson();

            // STEG 2: Fråga efter uppgifter
            var name = AnsiConsole.Ask<string>("Enter your [green]name[/]:");
            // Fråga efter e-post och fortsätt fråga tills den innehåller @ och punkt
            var email = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter your [green]email[/]:")
                    .Validate(e => AuthService.IsValidEmail(e)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Email must contain @ and a dot[/]")));

            // STEG 3: Kolla om e-posten redan används
            if (stafflist.FindItem(s => s.Email == email) != null)
            {
                MenuHelper.ShowError("This email is already registered.");
                return;   // avsluta metoden här
            }

            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("Choose a [green]password[/]:").Secret());

            // STEG 4: Skapa Staff-objektet och lägg till i listan
            var staff = new Staff { Name = name, Email = email, Password = password };
            stafflist.AddItem(staff);

            // STEG 5: Spara hela listan till JSON-filen
            stafflist.SaveToJson();
        }
        // Visar personalens arbetsschema.
        public void WorkingSchedule()
        {
            // Kontrollerar om personalen är inloggad.
            if (!IsLoggedIn)
            {
                MenuHelper.ShowError("You must be logged in to view the work schedule.");
                return;
            }

            // Kontrollerar om det finns några arbetspass.
            if (Schedule.WorkDays.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No work schedule is available.[/]");
                return;
            }

            // Skapar en tabell för arbetsschemat.
            var table = new Table();

            // Lägger till rubriker i tabellen.
            table.AddColumn("Day");
            table.AddColumn("Working Hours");

            // Går igenom alla arbetspass i schemat.
            foreach (string day in Schedule.WorkDays)
            {
                // Delar upp veckodag och arbetstid.
                string[] scheduleInfo = day.Split(' ', 2);

                // Lägger till arbetspasset i tabellen.
                table.AddRow(scheduleInfo[0], scheduleInfo[1]);
            }

            // Visar arbetsschemat i terminalen.
            AnsiConsole.Write(table);
        }

    }
}
