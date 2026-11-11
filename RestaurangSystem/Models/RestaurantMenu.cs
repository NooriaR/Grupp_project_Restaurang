using RestaurangSystem.Services;
using RestaurangSystem.UI;
using Spectre.Console;


namespace RestaurangSystem.Models
{
    public class RestaurantMenu
    {
        //Attributes
        public List<MenuItem> MenuItems = new List<MenuItem>();
        //Methods
        public void AddMenuItem(DataStore<MenuItem> dataStore)
        {
            
            // Skapar ett nytt maträttsobjekt
            MenuItem newItem = MenuItem.CreateMenuItem();


            // Lägger till maträtten i listan
            dataStore.AddItem(newItem);

            AnsiConsole.WriteLine($"{newItem.Name} har lagts till i menyn!");
        }
        // Removes a menu item from the data store by its Id
        public void RemoveMenuItem(DataStore<MenuItem> dataStore)
        {
            try
            {
                dataStore.GetIds(); // Show the list of items with their Ids
                                    // Ask the user which item to remove
                                    // STEP 1: Let the user choose how to search (arrow keys + Enter)
                var searchBy = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("How do you want to find the item?")
                        .AddChoices("Id", "Name"));

                MenuItem? item;   // "?" means it can be empty (null) if nothing is found

                // STEP 2: Ask for the id or the name, depending on the choice
                if (searchBy == "Id")
                {
                    int id = AnsiConsole.Ask<int>("Enter the [green]id[/]:");
                    item = dataStore.FindItem(m => m.Id == id);
                }
                else
                {
                    string name = AnsiConsole.Ask<string>("Enter the [green]name[/]:");
                    // OrdinalIgnoreCase means "pizza" and "Pizza" count as the same
                    item = dataStore.FindItem(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                }

                // STEP 3: Stop if nothing was found
                if (item == null)
                {
                    AnsiConsole.MarkupLine("[red]No item found.[/]");
                    return;
                }
        

                // Ask for confirmation before deleting
                if (!AnsiConsole.Confirm($"Remove [yellow]{Markup.Escape(item.Name)}[/]?"))
                {
                    AnsiConsole.MarkupLine("[yellow]Removal cancelled.[/]");
                    return;
                }

                dataStore.RemoveItem(item);
                AnsiConsole.MarkupLine("[green]Item removed![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Something went wrong: {Markup.Escape(ex.Message)}[/]");
            }
        }
        public void UppdateMenuItem(DataStore<MenuItem> dataStore)
        {
            try
            {
                var searchBy = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("How do you want to find the item?")
                    .AddChoices("Id", "Name"));

                MenuItem? item;   // "?" means it can be empty (null) if nothing is found
                if (searchBy == "Id")
                {
                    int id = AnsiConsole.Ask<int>("Enter the [green]id[/]:");
                    item = dataStore.FindItem(m => m.Id == id);
                }
                else
                {
                    string name = AnsiConsole.Ask<string>("Enter the [green]name[/]:");
                    // OrdinalIgnoreCase means "pizza" and "Pizza" count as the same
                    item = dataStore.FindItem(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                }

                // STEP 3: Stop if nothing was found
                if (item == null)
                {
                    AnsiConsole.MarkupLine("[red]No item found.[/]");
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

        public static void SortByPrice(DataStore<MenuItem> dataStore)
        {
            // Sortera från billigast till dyrast
            var sorted = dataStore.items.OrderBy(m => m.Price);

            // Skriv ut en rätt i taget
            foreach (var item in sorted)
            {
                AnsiConsole.WriteLine($"{item.Name} - {item.Price} kr");
            }
        }


        public static void ShowMenu(DataStore<MenuItem> menuStore)
        {
            bool running = true;

            while (running)
            {
                var choice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[green]What do you want to do?[/]")
                            .AddChoices("Show Menu", "Show Only Vegetarian", "Show By Category", "Sort By Price", "Back"));

                switch (choice)
                {
                    case "Show Menu":
                        MenuHelper.ShowMenuItems(menuStore);
                        break;

                    case "Show Only Vegetarian":
                        ShowOnlyVegetarian(menuStore);
                        break;

                    case "Show By Category":
                        ShowByCategory(menuStore);
                        break;

                    case "Sort By Price":
                        SortByPrice(menuStore);
                        break;
                    case "Back":
                        running = false;
                        break;
                }
            }
        }


        public static void ShowOnlyVegetarian(DataStore<MenuItem> dataStore)
        {
            // Välj ut bara de vegetariska rätterna
            var vegetarian = dataStore.items.Where(m => m.IsVegetarian);

            // Om inga hittades, säg det
            if (!vegetarian.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Inga vegetariska rätter hittades.[/]");
                return;
            }

            // Skriv ut en rätt i taget
            foreach (var item in vegetarian)
            {
                AnsiConsole.WriteLine($"{item.Name} - {item.Price} kr");
            }
        }
        public static void ShowByCategory(DataStore<MenuItem> dataStore)
        {
            // Låt användaren välja kategori med pilarna
            var category = AnsiConsole.Prompt(
                new SelectionPrompt<Category>()
                    .Title("Välj kategori:")
                    .AddChoices(Enum.GetValues<Category>()));

            // Välj ut bara rätterna i den kategorin
            var items = dataStore.items.Where(m => m.Category == category);

            // Om inga hittades, säg det
            if (!items.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Inga rätter hittades i den kategorin.[/]");
                return;
            }

            // Skriv ut en rätt i taget
            foreach (var item in items)
            {
                AnsiConsole.WriteLine($"{item.Name} - {item.Price} kr");
            }
        }

    }
}
