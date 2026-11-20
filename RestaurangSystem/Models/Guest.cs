
using RestaurangSystem.Services;
using RestaurangSystem.UI;
using Spectre.Console;

namespace RestaurangSystem.Models
{
    public class Guest : Users
    {
        // Egenskaper (Attributes)

        // Sparar en lista över gästens tidigare beställningar.
        public List<Order> Orderhistory { get; set; } = new List<Order>();

        // Sparar gästens reservation.
        public Reservation? reservation { get; set; }

        public void CreateOrder(List<MenuItem> menuItems)
        {
            DataStore<Order> orderList = new DataStore<Order>();
            orderList.LoadFromJson();

            // STEG 1: Visa menyn i en tabell
            var table = new Table();
            table.AddColumn("Dish");
            table.AddColumn("Price");

            foreach (var item in menuItems)
            {
                table.AddRow(item.Name, $"{item.Price} kr");
            }

            AnsiConsole.Write(table);

            // STEG 2: Skapa en ny beställning
            var order = new Order();

            // STEG 3: Låt gästen välja maträtter
            while (true)
            {
                string choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("What would you like to order?")
                        .AddChoices(menuItems.Select(m => m.Name).Append("Done")));

                if (choice == "Done")
                {
                    break;
                }

                MenuItem selected = menuItems.First(m => m.Name == choice);

                order.AddToOrder(selected);

                AnsiConsole.MarkupLine($"[green]{selected.Name} added to your order.[/]");
            }

            // STEG 4: Kontrollera om beställningen är tom
            if (order.OrderedItems.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Your order is empty.[/]");
                return;
            }

            // STEG 5: Lägg till gäster i beställningen
            int guestAmount = AnsiConsole.Prompt(
                new TextPrompt<int>("How many guests are included in this order?")
                    .DefaultValue(1)
                    .Validate(amount => amount > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("At least one guest is required.")));

            for (int i = 0; i < guestAmount; i++)
            {
                string guestName = AnsiConsole.Prompt(
                    new TextPrompt<string>($"Enter name for guest {i + 1}:"));

                Guest guest = new Guest
                {
                    Name = guestName
                };

                order.Guests.Add(guest);
            }

            // STEG 6: Spara beställningen
            orderList.AddItem(order);
            orderList.SaveToJson();

            Orderhistory.Add(order);

            // STEG 7: Visa beställningen
            var summary = new Table();
            summary.AddColumn("Dish");
            summary.AddColumn("Price");

            foreach (var item in order.OrderedItems)
            {
                summary.AddRow(item.Name, $"{item.Price} kr");
            }

            AnsiConsole.Write(summary);

            // Visa gästerna i beställningen
            AnsiConsole.MarkupLine("[bold]Guests:[/]");

            foreach (var guest in order.Guests)
            {
                AnsiConsole.MarkupLine($"- {Markup.Escape(guest.Name)}");
            }

            // STEG 8: Visa totalpriset
            var total = order.CalculateSumPrice();

            AnsiConsole.MarkupLine($"[bold]Total: {total} kr[/]");
            AnsiConsole.MarkupLine("[green]Thank you for your order![/]");
        }

        // Visar information om gästens nuvarande reservation.
        public void MakeReservation()
        {
            DataStore<Reservation> reservationDataStore = new DataStore<Reservation>();
            reservation = new Reservation();
            reservation.CreateReservation(reservationDataStore);
        }

        public void ShowReservation()
        {
            // Om gästen inte har någon bokning visas ett felmeddelande och metoden avslutas
            if (reservation == null)
            {
                AnsiConsole.MarkupLine("[red]You have no reservation.[/]");
                return;
            }

            // Skriver ut bokningens tid
            AnsiConsole.WriteLine($"Reservation time: {reservation.time}");

            // Skriver ut hur många personer bokningen gäller
            AnsiConsole.WriteLine($"Number of guests: {reservation.GuestAmount}");
        }

        // Visar status för gästens egna beställningar, en rad per beställning
        public void ShowOrderStatus()
        {
            // Om gästen inte har några beställningar visas ett meddelande och metoden avslutas
            if (Orderhistory.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]You have no orders.[/]");
                return;
            }

            // Går igenom gästens beställningar och skriver ut Id och status för varje
            foreach (Order order in Orderhistory)
            {
                AnsiConsole.WriteLine($"Order {order.Id}: {order.Status}");
            }
        }
        // Tar bort gästens reservation från datalagret.
        public void DeleteReservation()
        {
            // Skapar ett DataStore-objekt för att hantera reservationer.
            DataStore<Reservation> reservationDataStore = new DataStore<Reservation>();

            // Hämtar sparade reservationer från JSON-filen.
            reservationDataStore.LoadFromJson();

            // Kontrollera att gästen har en reservation innan vi använder dess ID.
            if (reservation == null)
            {
                return;
            }

            // Söker efter reservationen med hjälp av dess unika ID.
            Reservation? savedReservation = reservationDataStore.FindItem(r => r.Id == reservation.Id);

            // Kontrollerar om reservationen hittades i datalagret.
            if (savedReservation != null)
            {
                // Tar bort reservationen från datalagret.
                reservationDataStore.RemoveItem(savedReservation);

                // Sparar ändringarna i JSON-filen så att reservationen inte finns kvar.
                reservationDataStore.SaveToJson();
            }
        }
        // Avbokar gästens bokning genom att ta bort kopplingen mellan gästen och bokningen
        public void CancelReservation()
        {
            // Om gästen inte har någon bokning visas ett felmeddelande och metoden avslutas
            if (reservation == null)
            {
                AnsiConsole.MarkupLine("[red]You have no reservation to cancel.[/]");
                return;
            }

            // Tar bort bokningen från gästen genom att sätta den till null (ingenting)
            reservation = null;

            // Bekräftar för användaren att bokningen är avbokad
            AnsiConsole.MarkupLine("[green]Your reservation has been cancelled.[/]");
        }

        // Visar alla beställningar i en tabell, med den senaste beställningen först
        public void ShowOrderHistory(DataStore<Order> dataStore)
        {
            // try/catch fångar oväntade fel så att programmet inte kraschar
            try
            {
                // Om det inte finns några beställningar visas ett meddelande och metoden avslutas
                if (dataStore.items.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No orders found.[/]");
                    return;
                }

                // Sorterar beställningarna med LINQ så att den senaste kommer först
                var orders = dataStore.items.OrderByDescending(o => o.OrderTime);

                // Skapar tabellen och lägger till en kolumnrubrik för varje sorts information
                var table = new Table();
                table.AddColumn("Id");
                table.AddColumn("Time");
                table.AddColumn("Status");
                table.AddColumn("Items");
                table.AddColumn("Total");

                // Går igenom varje beställning och lägger till en rad i tabellen
                foreach (var order in orders)
                {
                    // Räknar ihop priset för alla rätter i beställningen
                    decimal total = order.OrderedItems.Sum(i => i.Price);

                    // Lägger till raden. Värdena måste komma i samma ordning som kolumnerna
                    table.AddRow(
                        order.Id.ToString(),
                        order.OrderTime.ToString("yyyy-MM-dd HH:mm"),   // visar tiden t.ex. 2026-11-10 12:30
                        order.Status.ToString(),
                        order.OrderedItems.Count.ToString(),            // antal rätter i beställningen
                        $"{total:F2} kr");                              // priset med två decimaler
                }

                // Skriver ut den färdiga tabellen
                AnsiConsole.Write(table);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error while showing order history: {ex.Message}", ex);
                // Visar felet i rött för användaren
                MenuHelper.ShowError("An unexpected error occurred while fetching the order history.");

            }
        }
    }
}

