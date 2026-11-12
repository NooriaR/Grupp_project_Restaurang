using RestaurangSystem.Services;
using Spectre.Console;

namespace RestaurangSystem.Models
{
    public class Guest : Users
    {
        // Egenskaper (Attributes)

        // Sparar en lista över gästens tidigare beställningar.
        // List<Order> innebär att listan kan innehålla flera beställningar.
        // new List<Order>() skapar en tom lista när ett nytt Guest-objekt skapas.
        public List<Order> Orderhistory { get; set; } = new List<Order>();

        // Sparar gästens reservation.
        // Reservation är datatypen och reservation är egenskapens namn.
        public Reservation reservation { get; set; }

        public void CreateOrder(List<MenuItem> menuItems)
        {
            {
                DataStore<Order> orderList = new DataStore<Order>();
                // ---------- STEG 1: Visa menyn i en tabell ----------
                var table = new Table();
                table.AddColumn("Dish");   // rubrik för kolumn 1
                table.AddColumn("Price");  // rubrik för kolumn 2

                // Gå igenom alla rätter i menyn och lägg till en rad för varje
                foreach (var item in menuItems)
                {
                    table.AddRow(item.Name, $"{item.Price} kr");
                }

                AnsiConsole.Write(table);  // rita ut tabellen i terminalen

                // ---------- STEG 2: Skapa en tom lista för gästens beställning ----------
                var order = new Order();  // skapa en ny Order-objekt som kommer att innehålla gästens valda rätter

                // ---------- STEG 3: Låt gästen välja rätt för rätt ----------
                while (true)  // fortsätt fråga tills gästen väljer "Done"
                {
                    // Visa en meny där gästen väljer med piltangenterna och Enter
                    string choice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("What would you like to order?")
                            .AddChoices(menuItems.Select(m => m.Name).Append("Done")));

                    // Om gästen väljer "Done" slutar vi fråga
                    if (choice == "Done")
                    {
                        break;
                    }

                    // Hitta rätten som har samma namn som gästen valde
                    MenuItem selected = menuItems.First(m => m.Name == choice);

                    // Lägg till rätten i beställningen
                    order.AddToOrder(selected);

                    AnsiConsole.MarkupLine($"[green]{selected.Name} added to your order.[/]");
                }

                // ---------- STEG 4: Om gästen inte valde något ----------
                if (order.OrderedItems.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]Your order is empty.[/]");
                    return;  // avsluta metoden här
                }
                orderList.AddItem(order);  // lägg till beställningen i listan över alla beställningar
                orderList.SaveToJson();  // spara listan till JSON-filen

                // ---------- STEG 5: Visa beställningen i en tabell ----------
                var summary = new Table();
                summary.AddColumn("Dish");
                summary.AddColumn("Price");

                foreach (var item in order.OrderedItems)
                {
                    summary.AddRow(item.Name, $"{item.Price} kr");
                }

                AnsiConsole.Write(summary);

                // ---------- STEG 6: Räkna ut och visa totalpriset ----------
                var total = order.CalculateSumPrice();  // Sum lägger ihop alla priser

                AnsiConsole.MarkupLine($"[bold]Total: {total} kr[/]");
                AnsiConsole.MarkupLine("[green]Thank you for your order![/]");
            }
        }


        // Metoder (Methods)

        // Visar information om gästens nuvarande reservation.

        public void MakeReservation()
        {
            DataStore<Reservation> reservationDataStore = new DataStore<Reservation>();
            reservation.CreateReservation(reservationDataStore);

        }
        public void ShowReservation()
        {
            // Kontrollerar om gästen har en reservation.
            if (reservation == null)
            {
                AnsiConsole.MarkupLine("[red]Du har ingen reservation.[/]");
                return;
            }

            // Visar datum och tid för reservationen.
            AnsiConsole.WriteLine($"Reservationstid: {reservation.time}");

            // Visar hur många gäster reservationen gäller.
            AnsiConsole.WriteLine($"Antal gäster: {reservation.GuestAmount}");
        }

        // Visar status för gästens alla beställningar.
        public void ShowOrderStatus()
        {
            // Kontrollerar om listan med beställningar är tom.
            if (Orderhistory.Count == 0)
            {
                // Skriver ut ett meddelande om gästen saknar beställningar.
                AnsiConsole.MarkupLine("[yellow]Du har inga beställningar.[/]");


                // Avslutar metoden direkt eftersom det inte finns något mer att visa.
                return;
            }

            // Loopar igenom alla beställningar i gästens orderhistorik.
            foreach (Order order in Orderhistory)
            {
                // Skriver ut varje beställnings ID och dess aktuella status.
                AnsiConsole.WriteLine($"Beställning {order.Id}: {order.Status}");
            }
        }

        // Avbryter gästens reservation.
        public void CancelReservation()
        {
            // Kontrollerar om gästen har en reservation.
            if (reservation == null)
            {
                AnsiConsole.MarkupLine("[red]Du har ingen reservation att avboka.[/]");
                return;
            }

            // Tar bort gästens koppling till reservationen.
            reservation = null;

            // Bekräftar att reservationen har avbokats.
            AnsiConsole.MarkupLine("[green]Din reservation har avbokats.[/]");
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