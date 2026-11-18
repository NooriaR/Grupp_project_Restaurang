
using RestaurangSystem.Services;
using Spectre.Console;

namespace RestaurangSystem.Models
{
    public class Guest : Users
    {
        // Egenskaper (Attributes)

        // Sparar en lista över gästens tidigare beställningar.
        public List<Order> Orderhistory { get; set; } = new List<Order>();

        // Sparar gästens reservation.
        public Reservation reservation { get; set; }

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
            if (reservation == null)
            {
                AnsiConsole.MarkupLine("[red]You have no reservation.[/]");
                return;
            }

            AnsiConsole.WriteLine($"Reservation time: {reservation.time}");
            AnsiConsole.WriteLine($"Number of guests: {reservation.GuestAmount}");
        }

        // Visar status för gästens alla beställningar.
        public void ShowOrderStatus()
        {
            if (Orderhistory.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]You have no orders.[/]");
                return;
            }

            foreach (Order order in Orderhistory)
            {
                AnsiConsole.WriteLine($"Order {order.Id}: {order.Status}");
            }
        }

        // Avbryter gästens reservation.
        public void CancelReservation()
        {
            if (reservation == null)
            {
                AnsiConsole.MarkupLine("[red]You have no reservation to cancel.[/]");
                return;
            }

            reservation = null;

            AnsiConsole.MarkupLine("[green]Your reservation has been cancelled.[/]");
        }

        public void ShowOrderHistory(DataStore<Order> dataStore)
        {
            try
            {
                if (dataStore.items.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No orders found.[/]");
                    return;
                }

                var orders = dataStore.items.OrderByDescending(o => o.OrderTime);

                var table = new Table();
                table.AddColumn("Id");
                table.AddColumn("Time");
                table.AddColumn("Status");
                table.AddColumn("Items");
                table.AddColumn("Total");

                foreach (var order in orders)
                {
                    decimal total = order.OrderedItems.Sum(i => i.Price);

                    table.AddRow(
                        order.Id.ToString(),
                        order.OrderTime.ToString("yyyy-MM-dd HH:mm"),
                        order.Status.ToString(),
                        order.OrderedItems.Count.ToString(),
                        $"{total:F2} kr");
                }

                AnsiConsole.Write(table);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Something went wrong: {Markup.Escape(ex.Message)}[/]");
            }
        }
    }
}

