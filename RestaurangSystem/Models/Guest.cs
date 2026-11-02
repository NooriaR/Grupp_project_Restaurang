using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RestaurangSystem.Models
{
    public class Guest : Users
    {
        //Attributes
        public string Namn { get; set; }
        public string Email { get; set; }
        public List<Order> Orderhistory { get; set; } = new List<Order>();
        public Reservation reservation { get; set; }

        //Methods 
        public void ShowReservation()
        {

        }
        public void ShowOrderStatus()
        {

        }
        public void CancelReservation()
        {

        }
    }
}
