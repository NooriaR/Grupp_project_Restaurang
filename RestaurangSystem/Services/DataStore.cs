using Spectre.Console;
using System.Text.Json;
namespace RestaurangSystem.Services
{
    public class DataStore<T> where T : IIdentify
    {
        public List<T> items = new List<T>();

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
        public void FindItem()
        {

        }
        public void GetItem()
        {

        }
        public void SaveToJson()
        {
            try
            {
                string json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText("data.json", json);
                AnsiConsole.MarkupLine("[green]Data sparades till data.json[/]");
            }
            catch (UnauthorizedAccessException)
            {
                AnsiConsole.MarkupLine("[red]Kunde inte spara: saknar behörighet till filen.[/]");
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
                string json = File.ReadAllText("data.json");
                items = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                AnsiConsole.MarkupLine($"[green]Läste in {items.Count} objekt från data.json[/]");
            }
            catch (FileNotFoundException)
            {
                AnsiConsole.MarkupLine("[yellow]Ingen sparad data hittades, startar med tom lista.[/]");
            }
            catch (JsonException)
            {
                AnsiConsole.MarkupLine("[red]Filen data.json är trasig eller har fel format. Startar med tom lista.[/]");
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