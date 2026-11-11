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
        public void WorkingSchedule()
        {

        }

    }
}



