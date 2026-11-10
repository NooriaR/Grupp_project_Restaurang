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
    }
}
