using Spectre.Console;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RestaurangSystem.Models
{
    public class Guest : Users
    {
        //Attributes
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Order> Orderhistory { get; set; } = new List<Order>();
        public Reservation reservation { get; set; }

        public void CreateOrder(List<MenuItem> menuItems)
        {
            {
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
    }
}
