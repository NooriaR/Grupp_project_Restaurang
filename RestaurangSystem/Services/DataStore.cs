using Spectre.Console;
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
        public void GetItem()
        {

        }
        public void SaveToJson()
        {

        }
        public void LoadFromJson()
        {

        }
    }
}