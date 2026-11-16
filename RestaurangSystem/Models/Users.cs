using RestaurangSystem.Services;
namespace RestaurangSystem.Models
{
    public class Users : IIdentify
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
    }
}
