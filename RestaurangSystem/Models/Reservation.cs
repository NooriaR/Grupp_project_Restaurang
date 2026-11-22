
using RestaurangSystem.Services;
using Spectre.Console;
using RestaurangSystem.UI;

namespace RestaurangSystem.Models
{
    public class Reservation : IIdentify
    {
        public Guest guest { get; set; }
        public DateTime time { get; set; }
        public int Id { get; set; }
        public int GuestAmount { get; set; }

        // Skapar en ny bokning och sparar den i JSON.
        public void CreateReservation(DataStore<Reservation> dataStore)
        {
            try
            {
                // Laddar tidigare reservationer från JSON.
                dataStore.LoadFromJson();

                // Hämtar gästens namn.
                string name = AnsiConsole.Ask<string>("Guest name:");

                // Hämtar datum och tid och kontrollerar att tiden ligger i framtiden.
                DateTime reservationTime = AnsiConsole.Prompt(
                    new TextPrompt<DateTime>("Date and time (for example 2026-11-20 18:00):")
                        .Validate(t => t > DateTime.Now
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]The time must be in the future[/]")));

                // Hämtar antal personer, minst 1.
                int numberOfGuests = AnsiConsole.Prompt(
                    new TextPrompt<int>("Number of guests:")
                        .Validate(n => n > 0
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]There must be at least 1 guest[/]")));

                // Sparar informationen i den aktuella reservationen.
                guest = new Guest { Name = name };
                time = reservationTime;
                GuestAmount = numberOfGuests;

                // Lägger till reservationen och sparar till JSON.
                dataStore.AddItem(this);
                dataStore.SaveToJson();

                AnsiConsole.MarkupLine($"[green]Reservation created for {Markup.Escape(name)}! Id: {Id}[/]");
            }
            catch (Exception ex)
            {
                // Visar ett tydligt felmeddelande för användaren.
                MenuHelper.ShowError("An unexpected error occurred while creating the reservation.");

                // Skriver felet till loggen.
                Logger.Error($"Error while creating reservation: {ex.Message}", ex);
            }
        }
    }
}
