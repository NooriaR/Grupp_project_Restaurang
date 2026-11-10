using RestaurangSystem.Services;
using Spectre.Console;

namespace RestaurangSystem.Models
{
    public class Guest : Users
    {
        //Attributes
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Order> Orderhistory { get; set; } = new List<Order>();
        public Reservation reservation { get; set; }

        //Methods 
        public void ShowReservation()
        {

        }
        // Visar status för gästens beställningar.
        public void ShowOrderStatus()
        {
            if (Orderhistory.Count == 0)
            {
                Console.WriteLine("Du har inga beställningar.");
                return;
            }

            foreach (Order order in Orderhistory)
            {
                Console.WriteLine($"Beställning {order.Id}: {order.Status}");
            }
        }
        public void CancelReservation()
        {

        }

        public void ShowOrderHistory(DataStore<Order> dataStore)
        {
            try
            {
                // Om det inte finns några beställningar, säg det
                if (dataStore.items.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No orders found.[/]");
                    return;
                }

                // Sortera så att den senaste beställningen kommer först
                var orders = dataStore.items.OrderByDescending(o => o.OrderTime);

                // Skapa tabellen
                var table = new Table();
                table.AddColumn("Id");
                table.AddColumn("Time");
                table.AddColumn("Status");
                table.AddColumn("Items");
                table.AddColumn("Total");

                foreach (var order in orders)
                {
                    // Räkna ihop priset för alla rätter i beställningen
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
