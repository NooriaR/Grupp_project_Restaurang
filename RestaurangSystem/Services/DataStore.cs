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

        }
        public void LoadFromJson()
        {

        }
    }
}