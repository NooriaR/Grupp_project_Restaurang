namespace RestaurangSystem.Services
{
    public class DataStore<T> where T : IIdentify
    {
        private List<T> items = new List<T>();
        public IReadOnlyList<T> Items => items;   
        public int Count => items.Count;

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

        }
        public void LoadFromJson()
        {

        }
    }
}
