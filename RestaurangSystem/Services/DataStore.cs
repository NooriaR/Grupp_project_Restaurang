using RestaurangSystem.UI;
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
                MenuHelper.ShowError("Error while searching");
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
                MenuHelper.ShowError("You do not have permission to access this file.");
            }
            catch (IOException ex)
            {
                MenuHelper.ShowError("Could not save the file.");
            }
            catch (Exception ex)
            {
                MenuHelper.ShowError("An unexpected error occurred while saving.");
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
                MenuHelper.ShowError("No saved data found, starting with an empty list.");
            }
            catch (JsonException)
            {
                MenuHelper.ShowError("The file is corrupted or has an invalid format. Starting with an empty list.");
                items = new List<T>();
            }
            catch (IOException ex)
            {
                MenuHelper.ShowError("Could not read the file.");
            }
            catch (Exception ex)
            {
                MenuHelper.ShowError("An unexpected error occurred while loading.");
            }
        }
    }
}
