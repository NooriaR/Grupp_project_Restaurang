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
        // Lägger till en ny maträtt i datalagret
        public void AddMenuItem(DataStore<MenuItem> dataStore)
        {

            // Skapar ett nytt maträttsobjekt
            MenuItem newItem = MenuItem.CreateMenuItem();


            // Lägger till maträtten i listan
            dataStore.AddItem(newItem);

            AnsiConsole.WriteLine($"{newItem.Name} has been added to the menu!");
        }
        // Tar bort en maträtt från datalagret med hjälp av dess 
        public void RemoveMenuItem(DataStore<MenuItem> dataStore)
        {
            try
            {
                dataStore.GetIds(); // Visar listan över maträtter med deras Id
                                    // Frågar användaren vilken maträtt som ska tas bort
                                    // STEG 1: Låt användaren välja hur maträtten ska sökas (arrow keys + Enter)
                var searchBy = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("How do you want to find the item?")
                        .AddChoices("Id", "Name"));

                MenuItem? item;   // "?" means it can be empty (null) if nothing is found

                // STEG 2: Fråga efter id eller namn, beroende på valet
                if (searchBy == "Id")
                {
                    int id = AnsiConsole.Ask<int>("Enter the [green]id[/]:");
                    item = dataStore.FindItem(m => m.Id == id);
                }
                else
                {
                    string name = AnsiConsole.Ask<string>("Enter the [green]name[/]:");
                    // OrdinalIgnoreCase betyder att "pizza" och "Pizza" räknas som samma
                    item = dataStore.FindItem(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                }

                // STEG 3: Avbryt om ingenting hittadest
                if (item == null)
                {
                    AnsiConsole.MarkupLine("[red]No item found.[/]");
                    return;
                }


                // Frågar om bekräftelse innan borttagning
                if (!AnsiConsole.Confirm($"Remove [yellow]{Markup.Escape(item.Name)}[/]?"))
                {
                    AnsiConsole.MarkupLine("[yellow]Removal cancelled.[/]");
                    return;
                }

                //Tar bort maträtten från listan och bekräftar för användaren
                dataStore.RemoveItem(item);

                AnsiConsole.MarkupLine("[green]Item removed![/]");
            }
            catch (Exception ex)
            {
                // Fångar oväntade fel så att programmet inte kraschar
                AnsiConsole.MarkupLine($"[red]Something went wrong: {Markup.Escape(ex.Message)}[/]");
                Logger.Error($"Error while removing menu item: {ex.Message}", ex);
            }
        }
        // Ändrar namn och pris på en maträtt som redan finns i menyn
        public void UppdateMenuItem(DataStore<MenuItem> dataStore)
        {
            try
            {
                var searchBy = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("How do you want to find the item?")
                    .AddChoices("Id", "Name"));

                MenuItem? item;   // "?" betyder att den kan vara tom (null) om inget hittas
                                  // Frågar efter id eller namn, beroende på valet
                if (searchBy == "Id")
                {
                    int id = AnsiConsole.Ask<int>("Enter the [green]id[/]:");
                    item = dataStore.FindItem(m => m.Id == id);
                }
                else
                {
                    string name = AnsiConsole.Ask<string>("Enter the [green]name[/]:");
                    // OrdinalIgnoreCase betyder att "pizza" och "Pizza" räknas som sammag
                    item = dataStore.FindItem(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                }

                // STEG 3: Avbryt om ingenting hittades
                if (item == null)
                {
                    AnsiConsole.MarkupLine("[red]No item found.[/]");
                    return;
                }
                // Ber användaren skriva in nytt namn och nytt pris
                item.Name = AnsiConsole.Ask<string>("New name:");
                item.Price = AnsiConsole.Ask<decimal>("New price:");

                AnsiConsole.MarkupLine("[green]Item updated![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Something went wrong: {ex.Message}[/]");
                Logger.Error($"Error while updating menu item: {ex.Message}", ex);
            }
        }
        // Skriver ut alla maträtter sorterade efter pris, billigast först
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

        // Visar en undermeny där gästen kan bläddra bland maträtterna
        public static void ShowMenu(DataStore<MenuItem> menuStore)
        {
            // Styr om menyn ska fortsätta visas
            bool running = true;

            while (running)
            {
                // Visar valen och sparar vad användaren väljer
                var choice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[green]What do you want to do?[/]")
                            .AddChoices("Show Menu", "Show Only Vegetarian", "Show By Category", "Sort By Price", "Back"));
                // Utför det användaren valde
                switch (choice)
                {
                    case "Show Menu":
                        // Visar alla maträtter i en tabell
                        MenuHelper.ShowMenuItems(menuStore);
                        break;

                    case "Show Only Vegetarian":
                        // Visar bara de vegetariska maträtterna
                        ShowOnlyVegetarian(menuStore);
                        break;

                    case "Show By Category":
                        // Visar maträtter i en vald kategori
                        ShowByCategory(menuStore);
                        break;

                    case "Sort By Price":
                        // Visar maträtterna sorterade efter pris
                        SortByPrice(menuStore);
                        break;
                    case "Back":
                        // Avslutar loopen och går tillbaka till föregående meny
                        running = false;
                        break;
                }
            }
        }

        // Skriver ut bara de maträtter som är vegetariska
        public static void ShowOnlyVegetarian(DataStore<MenuItem> dataStore)
        {
            // Välj ut bara de vegetariska rätterna
            var vegetarian = dataStore.items.Where(m => m.IsVegetarian);

            // Om inga hittades, säg det
            if (!vegetarian.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No vegetarian dishes found.[/]");
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
                    .Title("Select a category:")
                    .AddChoices(Enum.GetValues<Category>()));

            // Välj ut bara rätterna i den kategorin
            var items = dataStore.items.Where(m => m.Category == category);

            // Om inga hittades, säg det
            if (!items.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No dishes found in this category.[/]");
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
