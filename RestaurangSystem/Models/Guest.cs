using System;
using System.Collections.Generic;

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


        // Metoder (Methods)

        // Visar information om gästens nuvarande reservation.
        public void ShowReservation()
        {
            // Kontrollerar om gästen har en reservation.
            if (reservation == null)
            {
                Console.WriteLine("Du har ingen reservation.");
                return;
            }

            // Visar datum och tid för reservationen.
            Console.WriteLine($"Reservationstid: {reservation.time}");

            // Visar hur många gäster reservationen gäller.
            Console.WriteLine($"Antal gäster: {reservation.GuestAmount}");
        }

        // Visar status för gästens alla beställningar.
        public void ShowOrderStatus()
        {
            // Kontrollerar om listan med beställningar är tom.
            if (Orderhistory.Count == 0)
            {
                // Skriver ut ett meddelande om gästen saknar beställningar.
                Console.WriteLine("Du har inga beställningar.");

                // Avslutar metoden direkt eftersom det inte finns något mer att visa.
                return;
            }

            // Loopar igenom alla beställningar i gästens orderhistorik.
            foreach (Order order in Orderhistory)
            {
                // Skriver ut varje beställnings ID och dess aktuella status.
                Console.WriteLine($"Beställning {order.Id}: {order.Status}");
            }
        }

        // Avbryter gästens reservation.
        public void CancelReservation()
        {
            // Kontrollerar om gästen har en reservation.
            if (reservation == null)
            {
                Console.WriteLine("Du har ingen reservation att avboka.");
                return;
            }

            // Tar bort gästens koppling till reservationen.
            reservation = null;

            // Bekräftar att reservationen har avbokats.
            Console.WriteLine("Din reservation har avbokats.");
        }
    }
}