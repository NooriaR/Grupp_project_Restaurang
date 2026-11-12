using Spectre.Console;
using System.Text.Json;
namespace RestaurangSystem.Services
{
    public class DataStore<T> where T : IIdentify
    {
        public List<T> items = new List<T>();
        private static readonly string FilePath = $"{typeof(T).Name}.json"; // t.ex. Staff.json eller MenuItem.json


        public void AddItem(T item)
        {
            //sets a new id for every item added in a list, so no item has the same id
            item.Id = items.Select(i => i.Id).DefaultIfEmpty(0).Max() + 1;
            //add the item to the list
            items.Add(item);

        }
         public void RemoveItem(T item)
        {
            items.Remove(item);
        }
        // Finds the first item that matches a condition, returns null if nothing is found
        public T? FindItem(Func<T, bool> predicate)
        {
            try
            {
                return items.FirstOrDefault(predicate);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Fel vid sökning: {ex.Message}[/]");
                return default;
            }
        }
        public void GetIds()
        {
            foreach(var item in items)
            {
                AnsiConsole.MarkupLine($"[green]Item: {item.GetType().Name} Id: {item.Id}[/]");
            }
        }
        public void SaveToJson()
        {
            try
            {
                string json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
                AnsiConsole.MarkupLine($"[green]Data sparades till {FilePath}[/]");
            }
            catch (UnauthorizedAccessException)
            {
                AnsiConsole.MarkupLine($"[red]Kunde inte spara: saknar behörighet till {FilePath}.[/]");
            }
            catch (IOException ex)
            {
                AnsiConsole.MarkupLine($"[red]Kunde inte spara filen:[/] {Markup.Escape(ex.Message)}");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Oväntat fel vid sparning:[/] {Markup.Escape(ex.Message)}");
            }
        }

        public void LoadFromJson()
        {
            try
            {
                string json = File.ReadAllText(FilePath);
                items = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                AnsiConsole.MarkupLine($"[green]Läste in {items.Count} objekt från {FilePath}[/]");
            }
            catch (FileNotFoundException)
            {
                AnsiConsole.MarkupLine("[yellow]Ingen sparad data hittades, startar med tom lista.[/]");
            }
            catch (JsonException)
            {
                AnsiConsole.MarkupLine($"[red]Filen {FilePath} är trasig eller har fel format. Startar med tom lista.[/]");
                items = new List<T>();
            }
            catch (IOException ex)
            {
                AnsiConsole.MarkupLine($"[red]Kunde inte läsa filen:[/] {Markup.Escape(ex.Message)}");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Oväntat fel vid inläsning:[/] {Markup.Escape(ex.Message)}");
            }
        }
    }
}