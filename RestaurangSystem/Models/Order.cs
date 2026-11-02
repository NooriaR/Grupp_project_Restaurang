using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RestaurangSystem.Models
{
    public class Order
    {
        //Attributes
        public List<Order> Orders = new List<Order>();
        public List<Guest> Guests = new List<Guest>();
        // Enum OrderStatus Status behöver en enum class som heter OrderStatus
        public DateTime OrderTime;

        //Methods
        public void AddToOrder()
        {

        }
        public void RemoveFromOrder()
        {

        }
        public void CalculateSumPrice()
        {

        }
    }
}
