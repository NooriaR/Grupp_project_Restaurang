using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RestaurangSystem.Models
{
    public class RestaurantMenu
    {
        //Attributes
        public List<MenuItem> MenuItems = new List<MenuItem>();
        //Methods
        public void AddMenuItem()
        {
            // Hämtar maträttens namn
            Console.Write("Ange maträttens namn: ");
            string namn = Console.ReadLine();

            // Hämtar priset och kontrollerar att det är giltigt
            Console.Write("Ange pris: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal pris) || pris < 0)
            {
                Console.WriteLine("Ogiltigt pris!");
                return;
            }

            // Hämtar maträttens innehåll
            Console.Write("Ange innehåll: ");
            string innehall = Console.ReadLine();

            // Frågar om maträtten är vegetarisk
            Console.Write("Är maträtten vegetarisk? (ja/nej): ");
            string svar = Console.ReadLine().ToLower();

            bool isVegeterian;

            if (svar == "ja")
            {
                isVegeterian = true;
            }
            else if (svar == "nej")
            {
                isVegeterian = false;
            }
            else
            {
                Console.WriteLine("Svara ja eller nej!");
                return;
            }

            // Hämtar maträttens kategori
            Console.Write("Ange kategori: ");
            string category = Console.ReadLine();

            // Skapar ett nytt maträttsobjekt
            MenuItem newItem = new MenuItem
            {
                Namn = namn,
                Pris = pris,
                innehåll = innehall,
                isVegeterian = isVegeterian
            };

            // Lägger till maträtten i listan
            MenuItems.Add(newItem);

            Console.WriteLine($"{namn} har lagts till i menyn!");
        }
        public void RemoveMenuItem()
        {
            
        }   
        public void SortByPrice()
        {

        }
        public void ShowMenu()
        {

        }
        public void ShowOnlyVegetarian()
        {

        }
        public void ShowByCategory()
        {

        }

    }
}
