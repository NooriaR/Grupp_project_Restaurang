using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurangSystem.Models
{
    public class MenuItem
    {
        public string Namn { get; set; }
        public decimal Pris { get; set; }
        public string innehåll { get; set; }
        public bool isVegeterian { get; set; }

        //This needs an enum class called Category to not throw error
        //Public Category category
    }
}
