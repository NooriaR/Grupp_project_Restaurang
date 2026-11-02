using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurangSystem.Models
{
    public class Reservation
    {
        public Guest guest { get; set; }
        public DateTime time { get; set; }
        public int GuestAmount { get; set; }
    }
}
