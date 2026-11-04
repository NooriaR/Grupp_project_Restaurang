using RestaurangSystem.Services;

namespace RestaurangSystem.Models
{
    public enum Category
    {
        Förrätt,
        Huvudrätt,
        Efterrätt,
        Dryck
    }

    public class MenuItem : IIdentify
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Content { get; set; }
        public bool IsVegetarian { get; set; }
        public Category Category { get; set; }
    }
}
