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
                AnsiConsole.MarkupLine($"[red]Error while searching: {ex.Message}[/]");
                Logger.Error($"Error while searching for item: {ex.Message}", ex);
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
                AnsiConsole.MarkupLine($"[green]Data saved to {FilePath}[/]");
            }
            catch (UnauthorizedAccessException)
            {
                AnsiConsole.MarkupLine($"[red]Could not save: no permission to access {FilePath}.[/]");
                Logger.Error($"UnauthorizedAccessException while saving to {FilePath}");
            }
            catch (IOException ex)
            {
                AnsiConsole.MarkupLine($"[red]Could not save the file:[/] {Markup.Escape(ex.Message)}");
                Logger.Error($"IOException while saving to {FilePath}", ex);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Unexpected error while saving:[/] {Markup.Escape(ex.Message)}");
                Logger.Error($"Unexpected error while saving to {FilePath}", ex);
            }
        }

        public void LoadFromJson()
        {
            try
            {
                string json = File.ReadAllText(FilePath);
                items = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                AnsiConsole.MarkupLine($"[green]Loaded {items.Count} objects from {FilePath}[/]");
            }
            catch (FileNotFoundException)
            {
                AnsiConsole.MarkupLine("[yellow]No saved data found, starting with an empty list.[/]");
                Logger.Warning($"No saved data found for {FilePath}");
            }
            catch (JsonException)
            {
                AnsiConsole.MarkupLine($"[red]The file {FilePath} is corrupted or has an invalid format. Starting with an empty list.[/]");
                Logger.Warning($"JsonException while loading from {FilePath}");
                items = new List<T>();
            }
            catch (IOException ex)
            {
                AnsiConsole.MarkupLine($"[red]Could not read the file:[/] {Markup.Escape(ex.Message)}");
                Logger.Error($"IOException while loading from {FilePath}", ex);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Unexpected error while loading:[/] {Markup.Escape(ex.Message)}");
                Logger.Error($"Unexpected error while loading from {FilePath}", ex);
            }
        }
    }
}
