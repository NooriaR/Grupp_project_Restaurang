
using RestaurangSystem.Services;
using Spectre.Console;

namespace RestaurangSystem.Models
{
    public class Guest : Users
    {
        // Sparar gästens tidigare beställningar.
        public List<Order> Orderhistory { get; set; } = new List<Order>();

        // Sparar gästens reservation.
        public Reservation? reservation { get; set; }

        // Skapar en ny beställning.
        public void CreateOrder(List<MenuItem> menuItems)
        {
            DataStore<Order> orderList = new DataStore<Order>();
            orderList.LoadFromJson();

            // Visar menyn i en tabell.
            var table = new Table();
            table.AddColumn("Dish");
            table.AddColumn("Price");

            foreach (var item in menuItems)
            {
                table.AddRow(item.Name, $"{item.Price} kr");
            }

            AnsiConsole.Write(table);

            // Skapar en ny beställning.
            var order = new Order();

            // Låter gästen välja maträtter.
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

            // Kontrollerar att beställningen inte är tom.
            if (order.OrderedItems.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Your order is empty.[/]");
                return;
            }

            // Låter gästen lägga till personer i beställningen.
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

            // Lägger till beställningen och sparar till JSON.
            orderList.AddItem(order);
            orderList.SaveToJson();

            Orderhistory.Add(order);

            // Visar beställningen.
            var summary = new Table();
            summary.AddColumn("Dish");
            summary.AddColumn("Price");

            foreach (var item in order.OrderedItems)
            {
                summary.AddRow(item.Name, $"{item.Price} kr");
            }

            AnsiConsole.Write(summary);

            // Visar gästerna i beställningen.
            AnsiConsole.MarkupLine("[bold]Guests:[/]");

            foreach (var guest in order.Guests)
            {
                AnsiConsole.MarkupLine($"- {Markup.Escape(guest.Name)}");
            }

            // Räknar ut totalpriset.
            var total = order.CalculateSumPrice();

            AnsiConsole.MarkupLine($"[bold]Total: {total} kr[/]");
            AnsiConsole.MarkupLine("[green]Thank you for your order![/]");
        }

        // Skapar en ny reservation.
        public void MakeReservation()
        {
            DataStore<Reservation> reservationDataStore = new DataStore<Reservation>();

            reservation = new Reservation();
            reservation.CreateReservation(reservationDataStore);
        }

        // Hämtar gästens reservation från JSON.
        public void LoadReservation()
        {
            DataStore<Reservation> reservationDataStore = new DataStore<Reservation>();
            reservationDataStore.LoadFromJson();

            reservation = reservationDataStore.FindItem(
                r => string.Equals(
                    r.guest?.Name,
                    Name,
                    StringComparison.OrdinalIgnoreCase));
        }

        // Visar gästens reservation.
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

        // Visar status för alla gästers beställningar.
        public void ShowOrderStatus()
        {
            DataStore<Order> orderDataStore = new DataStore<Order>();
            orderDataStore.LoadFromJson();

            if (orderDataStore.items.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]You have no orders.[/]");
                return;
            }

            foreach (Order order in orderDataStore.items)
            {
                AnsiConsole.WriteLine($"Order {order.Id}: {order.Status}");
            }
        }

        // Tar bort gästens reservation från datalagret.
        public void DeleteReservation()
        {
            DataStore<Reservation> reservationDataStore = new DataStore<Reservation>();

            reservationDataStore.LoadFromJson();

            if (reservation == null)
            {
                return;
            }

            Reservation? savedReservation = reservationDataStore.FindItem(
                r => r.Id == reservation.Id);

            if (savedReservation != null)
            {
                reservationDataStore.RemoveItem(savedReservation);
                reservationDataStore.SaveToJson();
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

            DeleteReservation();

            reservation = null;

            AnsiConsole.MarkupLine("[green]Your reservation has been cancelled.[/]");
        }

        // Visar alla sparade beställningar.
        public void ShowOrderHistory(DataStore<Order> dataStore)
        {
            try
            {
                // Laddar beställningar från JSON.
                dataStore.LoadFromJson();

                if (dataStore.items.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No orders found.[/]");
                    return;
                }

                // Sorterar så att den senaste beställningen visas först.
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
                AnsiConsole.MarkupLine(
                    $"[red]Something went wrong: {Markup.Escape(ex.Message)}[/]");
            }
        }
    }
}
