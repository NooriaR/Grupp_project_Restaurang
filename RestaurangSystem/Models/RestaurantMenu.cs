using RestaurangSystem.Services;
using Spectre.Console;
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
            AnsiConsole.Write("Ange maträttens namn: ");
            string namn = Console.ReadLine();

            // Hämtar priset och kontrollerar att det är giltigt
            AnsiConsole.Write("Ange pris: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal pris) || pris < 0)
            {
                AnsiConsole.WriteLine("Ogiltigt pris!");
                return;
            }

            // Hämtar maträttens innehåll
            AnsiConsole.Write("Ange innehåll: ");
            string innehall = Console.ReadLine();

            // Frågar om maträtten är vegetarisk
            AnsiConsole.Write("Är maträtten vegetarisk? (ja/nej): ");
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
            AnsiConsole.Write("Ange kategori: ");
            string category = Console.ReadLine();

            // Skapar ett nytt maträttsobjekt
            MenuItem newItem = new MenuItem
            {
                Name = namn,
                Price = pris,
                Content = innehall,
                IsVegetarian = isVegeterian,
                Category = Enum.Parse<Category>(category),
            };

            // Lägger till maträtten i listan
            MenuItems.Add(newItem);

            AnsiConsole.WriteLine($"{namn} har lagts till i menyn!");
        }
        public void RemoveMenuItem()
        {

        }

        public void UppdateMenuItem(DataStore<MenuItem> dataStore)
        {
            try
            {
                int id = AnsiConsole.Ask<int>("Enter the Id of the item to update:");

                MenuItem item = dataStore.FindItem(m => m.Id == id);

                if (item == null)
                {
                    AnsiConsole.MarkupLine("[red]Item not found.[/]");
                    return;
                }

                item.Name = AnsiConsole.Ask<string>("New name:");
                item.Price = AnsiConsole.Ask<decimal>("New price:");

                AnsiConsole.MarkupLine("[green]Item updated![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Something went wrong: {ex.Message}[/]");
            }
        }
        
        public void SortByPrice()
        {

        }

        public static void ShowMenu()
        {
            bool running = true;

            while (running)
            {
                var choice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[green]What do you want to do?[/]")
                            .AddChoices("Add MenuItem", "Show Menu", "Finish"));

                switch (choice)
                {
                    case "Add MenuItem":
                        Console.WriteLine("This is the AddMenuItem()");
                        break;

                    case "Show Menu":
                        Console.WriteLine("This is the ShowMenuItems()");
                        break;

                    case "Finish":
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
