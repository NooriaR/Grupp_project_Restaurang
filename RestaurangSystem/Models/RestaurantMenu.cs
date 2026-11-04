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
