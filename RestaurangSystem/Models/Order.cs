using RestaurangSystem.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RestaurangSystem.Models
{
    public class Order : IIdentify
    {
        //Attributes
        public int Id { get; set; }
        public List<MenuItem> OrdereredItems = new List<MenuItem>();
        public List<Guest> Guests = new List<Guest>();
        public DateTime OrderTime;

        
        public void RemoveFromOrder(int id)
        {
            var item = OrdereredItems.Find(menuitem => menuitem.Id == id);
            OrdereredItems.Remove(item);
        
        }
        public void CalculateSumPrice()
        {

        }
    }
}
