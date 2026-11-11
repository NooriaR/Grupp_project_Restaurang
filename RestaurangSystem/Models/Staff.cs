using RestaurangSystem.Services;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RestaurangSystem.Models
{
    public class Staff : Users
    {
        //Attributes
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsLoggedIn { get; set; }

       //methods
       
        public void Register()
        {
            string file = "staff.json";   // filen där alla staff sparas

            // STEG 1: Ladda de som redan är sparade (om filen finns)
            var staffList = new List<Staff>();
            if (File.Exists(file))
            {
                string json = File.ReadAllText(file);
                staffList = JsonSerializer.Deserialize<List<Staff>>(json) ?? new List<Staff>();
            }

            // STEG 2: Fråga efter uppgifter
            var name = AnsiConsole.Ask<string>("Enter your [green]name[/]:");
            // Fråga efter e-post och fortsätt fråga tills den innehåller @ och punkt
            var email = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter your [green]email[/]:")
                    .Validate(e => AuthService.IsValidEmail(e)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Email must contain @ and a dot[/]")));

            // STEG 3: Kolla om e-posten redan används
            if (staffList.Any(s => s.Email == email))
            {
                AnsiConsole.MarkupLine("[red]This email is already registered.[/]");
                return;   // avsluta metoden här
            }

            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("Choose a [green]password[/]:").Secret());

            // STEG 4: Skapa Staff-objektet och lägg till i listan
            var staff = new Staff { Name = name, Email = email, Password = password };
            staffList.Add(staff);

            // STEG 5: Spara hela listan till JSON-filen
            try
            {
                string newJson = JsonSerializer.Serialize(staffList,
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(file, newJson);

                AnsiConsole.MarkupLine($"[green]Welcome, {name}! You are now registered.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Could not save: {ex.Message}[/]");
            }
        }
        public void WorkingSchedule()
        {

        }

    }
}



