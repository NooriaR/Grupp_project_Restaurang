using RestaurangSystem.Services;
using Spectre.Console;

namespace RestaurangSystem.Models
{
    public class Reservation : IIdentify
    {
        public Guest guest { get; set; }
        public DateTime time { get; set; }
        public int Id { get; set; }
        public int GuestAmount { get; set; }


        public void CreateReservation(DataStore<Reservation> dataStore)
        {
            try
            {
                // Hämtar gästens namn
                string name = AnsiConsole.Ask<string>("Guest name:");

                // Hämtar datum och tid, och stoppar tider som redan har varit
                DateTime time = AnsiConsole.Prompt(
                    new TextPrompt<DateTime>("Date and time (for example 2026-11-20 18:00):")
                        .Validate(t => t > DateTime.Now
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]The time must be in the future[/]")));

                // Hämtar antal personer, minst 1
                int numberOfGuests = AnsiConsole.Prompt(
                    new TextPrompt<int>("Number of guests:")
                        .Validate(n => n > 0
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]There must be at least 1 guest[/]")));

                // Skapar bokningen
                Reservation reservation = new Reservation
                {
                    guest = new Guest { Name = name },
                    time = time,
                    GuestAmount = numberOfGuests
                };

                // AddItem sätter Id automatiskt
                dataStore.AddItem(reservation);
                dataStore.SaveToJson();

                AnsiConsole.MarkupLine($"[green]Reservation created for {Markup.Escape(name)}! Id: {reservation.Id}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Something went wrong: {Markup.Escape(ex.Message)}[/]");
            }

        }
    }
}

