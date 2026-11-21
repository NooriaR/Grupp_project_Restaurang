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


        // Skapar en ny bokning: frågar efter gästens namn, tid och antal personer,
        // validerar svaren och sparar bokningen i datalagret och i JSON-filen
        public void CreateReservation(DataStore<Reservation> dataStore)
        {
            // try/catch fångar oväntade fel så att programmet inte kraschar
            try
            {
                // Frågar efter gästens namn (Spectre upprepar frågan om svaret är tomt)
                string name = AnsiConsole.Ask<string>("Guest name:");

                // Frågar efter datum och tid
                // Validate stoppar tider som redan har varit, och frågan ställs om vid fel
                DateTime time = AnsiConsole.Prompt(
                    new TextPrompt<DateTime>("Date and time (for example 2026-11-20 18:00):")
                        .Validate(t => t > DateTime.Now
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]The time must be in the future[/]")));

                // Frågar efter antal gäster
                // Validate kräver minst 1 person, och text i stället för tal avvisas av Spectre
                int numberOfGuests = AnsiConsole.Prompt(
                    new TextPrompt<int>("Number of guests:")
                        .Validate(n => n > 0
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]There must be at least 1 guest[/]")));

                // Skapar ett nytt bokningsobjekt av svaren som användaren har gett
                Reservation reservation = new Reservation
                {
                    guest = new Guest { Name = name },
                    time = time,
                    GuestAmount = numberOfGuests
                };

                // Lägger in bokningen i datalagret. AddItem ger bokningen ett nytt Id automatiskt
                dataStore.AddItem(reservation);

                // Sparar alla bokningar till JSON-filen så att de finns kvar nästa gång programmet startas
                dataStore.SaveToJson();

                // Bekräftar för användaren och visar bokningens Id
                // Markup.Escape skyddar mot tecken i namnet som kan förstöra färgkoderna
                AnsiConsole.MarkupLine($"[green]Reservation created for {Markup.Escape(name)}! Id: {reservation.Id}[/]");
            }
            catch (Exception ex) // Fångar eventuella fel som kan uppstå under skapandet av bokningen
            {
                // Visar ett tydligt felmeddelande för användaren
                MenuHelper.ShowError("An unexpected error occurred while creating the reservation.");

                // Skriver felet till loggen så att gruppen kan se vad som gick fel
                Logger.Error($"Error while creating reservation: {ex.Message}", ex);
            }
        }
    }
}

