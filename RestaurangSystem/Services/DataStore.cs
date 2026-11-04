using RestaurangSystem.Models;
using System.Text.Json;

namespace RestaurangSystem.Services
{
    public class DataStore<T> where T : IIdentify
    {
        List<T> items = new List<T>();

        public void AddItem(T item)
        {
            //sets a new id for every item added in a list, so no item has the same id
            item.Id = items.Select(i => i.Id).DefaultIfEmpty(0).Max() + 1;

        }
        public void RemoveItem()
        {

        }
        public void FindItem()
        {

        }
        public void GetItem()
        {

        }
        public void SaveToJson()
        {
            string json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("data.json", json);
        }
        public void LoadFromJson()
        {
            if (!File.Exists("data.json"))
            {
                return;
            }

            string json = File.ReadAllText("data.json");
            items = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
    }
    
}
