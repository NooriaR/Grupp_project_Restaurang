using RestaurangSystem.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurangSystem.Models
{
    public class Users : IIdentify
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
    }
}
