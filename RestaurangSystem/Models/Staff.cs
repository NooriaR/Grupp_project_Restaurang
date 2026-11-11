using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RestaurangSystem.Models
{
    public class Staff : Users
    {
        //Attributes
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [JsonIgnore]
        public bool IsLoggedIn { get; set; }
        //Methods
        public void WorkingSchedule()
        {

        }
    }
}



