using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Spectre.Console;

namespace RestaurangSystem.Models
{
    public class RestaurantMenu
    {
        //Attributes
        public List<MenuItem> MenuItems = new List<MenuItem>();
        //Methods
        public void AddMenuItem()
        {

        }
        public void RemoveMenuItem()
        {
            
        }   
        public void SortByPrice()
        {

        }

        static void ShowMenu()
            {
                bool running = true;

                while (running)
                {
                    var choice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[green]Vad vill du göra?[/]")
                            .AddChoices("Lägg till rätt", "Visa menyn", "Avsluta"));

                    switch (choice)
                    {
                        case "Lägg till rätt":
                            Console.WriteLine("Här kommer AddMenuItem()");
                            break;

                        case "Visa menyn":
                            Console.WriteLine("Här kommer ShowMenuItems()");
                            break;

                        case "Avsluta":
                            running = false;
                            break;
                    }
                }
            }

           
        public void ShowOnlyVegetarian()
        {

        }
        public void ShowByCategory()
        {

        }

    }
}
