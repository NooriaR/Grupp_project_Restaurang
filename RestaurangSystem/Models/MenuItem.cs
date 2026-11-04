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
        public string Namn { get; set; }
        public decimal Pris { get; set; }
        public string innehåll { get; set; }
        public bool isVegeterian { get; set; }
        public Category category { get; set; }
    }
}
