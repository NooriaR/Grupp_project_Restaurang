using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurangSystem.Models
{
    public class Staff : Users
    {
        //Attributes
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsLoggedIn { get; set; }

        //Methods
        public void WorkingSchedule()
        {

        }
    }
}



