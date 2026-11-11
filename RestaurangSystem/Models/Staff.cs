using Spectre.Console;
using System.Text.Json.Serialization;

﻿using RestaurangSystem.Services;
using Spectre.Console;
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
                AnsiConsole.MarkupLine("[red]This email is already registered.[/]");
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
                AnsiConsole.MarkupLine("[red]Du måste vara inloggad för att se arbetsschemat.[/]");
                return;
            }

            // Kontrollerar om det finns några arbetspass.
            if (Schedule.WorkDays.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Inget arbetsschema finns tillgängligt.[/]");
                return;
            }

            // Skapar en tabell för arbetsschemat.
            var table = new Table();

            // Lägger till rubriker i tabellen.
            table.AddColumn("Dag");
            table.AddColumn("Arbetstid");

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