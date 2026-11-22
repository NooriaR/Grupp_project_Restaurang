using RestaurangSystem.Services;
using RestaurangSystem.UI;
using Spectre.Console;

namespace RestaurangSystem.Models
{
    public class RestaurantMenu
    {
        // Attribut
        public List<MenuItem> MenuItems = new List<MenuItem>();

        // Metoder

        // Lägger till en ny maträtt i datalagret.
        public void AddMenuItem(DataStore<MenuItem> dataStore)
        {
            // Laddar tidigare maträtter från JSON.
            dataStore.LoadFromJson();

            // Skapar ett nytt maträttsobjekt.
            MenuItem newItem = MenuItem.CreateMenuItem();

            // Lägger till maträtten i listan.
            dataStore.AddItem(newItem);

            // Sparar hela listan till JSON.
            dataStore.SaveToJson();

            AnsiConsole.WriteLine($"{newItem.Name} has been added to the menu!");
        }

        // Tar bort en maträtt från datalagret.
        public void RemoveMenuItem(DataStore<MenuItem> dataStore)
        {
            try
            {
                // Laddar tidigare maträtter från JSON.
                dataStore.LoadFromJson();

                // Visar listan med maträtter och deras ID.
                dataStore.GetIds();

                // STEG 1: Låt användaren välja hur maträtten ska sökas.
                var searchBy = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("How do you want to find the item?")
                        .AddChoices("Id", "Name"));

                MenuItem? item;

                // Frågar efter ID eller namn beroende på valet.
                if (searchBy == "Id")
                {
                    int id = AnsiConsole.Ask<int>("Enter the [green]id[/]:");
                    item = dataStore.FindItem(m => m.Id == id);
                }
                else
                {
                    string name = AnsiConsole.Ask<string>("Enter the [green]name[/]:");
                    // OrdinalIgnoreCase gör att stora och små bokstäver behandlas lika.
                    item = dataStore.FindItem(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                }

                // Avbryter om ingen maträtt hittades.
                if (item == null)
                {
                    AnsiConsole.MarkupLine("[red]No item found.[/]");
                    return;
                }

                // Frågar om bekräftelse innan borttagning.
                if (!AnsiConsole.Confirm($"Remove [yellow]{Markup.Escape(item.Name)}[/]?"))
                {
                    AnsiConsole.MarkupLine("[yellow]Removal cancelled.[/]");
                    return;
                }

                // Tar bort maträtten från listan.
                dataStore.RemoveItem(item);

                // Sparar ändringen till JSON.
                dataStore.SaveToJson();

                AnsiConsole.MarkupLine("[green]Item removed![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Something went wrong: {Markup.Escape(ex.Message)}[/]");
                Logger.Error($"Error while removing menu item: {ex.Message}", ex);
            }
        }

        // Ändrar namn och pris på en maträtt som redan finns i menyn.
        public void UppdateMenuItem(DataStore<MenuItem> dataStore)
        {
            try
            {
                dataStore.LoadFromJson();

                var searchBy = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("How do you want to find the item?")
                        .AddChoices("Id", "Name"));

                MenuItem? item;

                // Frågar efter ID eller namn beroende på valet.
                if (searchBy == "Id")
                {
                    int id = AnsiConsole.Ask<int>("Enter the [green]id[/]:");
                    item = dataStore.FindItem(m => m.Id == id);
                }
                else
                {
                    string name = AnsiConsole.Ask<string>("Enter the [green]name[/]:");
                    item = dataStore.FindItem(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                }

                // Avbryter om ingen maträtt hittades.
                if (item == null)
                {
                    AnsiConsole.MarkupLine("[red]No item found.[/]");
                    return;
                }

                // Ber användaren skriva in nytt namn och nytt pris.
                item.Name = AnsiConsole.Ask<string>("New name:");
                item.Price = AnsiConsole.Ask<decimal>("New price:");

                AnsiConsole.MarkupLine("[green]Item updated![/]");

                // Sparar ändringen till JSON.
                dataStore.SaveToJson();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Something went wrong: {Markup.Escape(ex.Message)}[/]");
                Logger.Error($"Error while updating menu item: {ex.Message}", ex);
            }
        }

        // Skriver ut alla maträtter sorterade efter pris, billigast först.
        public static void SortByPrice(DataStore<MenuItem> dataStore)
        {
            var sorted = dataStore.items.OrderBy(m => m.Price);

            foreach (var item in sorted)
            {
                AnsiConsole.WriteLine($"{item.Name} - {item.Price} kr");
            }
        }

        // Visar en undermeny där gästen kan bläddra bland maträtterna.
        public static void ShowMenu(DataStore<MenuItem> menuStore)
        {
            bool running = true;

            // Laddar maträtterna från JSON.
            menuStore.LoadFromJson();

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

        // Skriver ut bara de maträtter som är vegetariska.
        public static void ShowOnlyVegetarian(DataStore<MenuItem> dataStore)
        {
            var vegetarian = dataStore.items.Where(m => m.IsVegetarian);

            if (!vegetarian.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No vegetarian dishes found.[/]");
                return;
            }

            foreach (var item in vegetarian)
            {
                AnsiConsole.WriteLine($"{item.Name} - {item.Price} kr");
            }
        }

        // Visar maträtter i en vald kategori.
        public static void ShowByCategory(DataStore<MenuItem> dataStore)
        {
            var category = AnsiConsole.Prompt(
                new SelectionPrompt<Category>()
                    .Title("Select a category:")
                    .AddChoices(Enum.GetValues<Category>()));

            var items = dataStore.items.Where(m => m.Category == category);

            if (!items.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No dishes found in this category.[/]");
                return;
            }

            foreach (var item in items)
            {
                AnsiConsole.WriteLine($"{item.Name} - {item.Price} kr");
            }
        }
    }
}
