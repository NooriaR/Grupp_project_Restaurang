
using RestaurangSystem.Services;

namespace RestaurangSystem.Models;

// Representerar en beställning i restaurangen.
public class Order : IIdentify
{
    // Ett unikt nummer för beställningen.
    public int Id { get; set; }

    // Sparar alla maträtter som ingår i beställningen.
    public List<MenuItem> OrderedItems { get; set; } = new List<MenuItem>();

    // Sparar gäster som hör till beställningen.
    public List<Guest> Guests { get; set; } = new List<Guest>();

    // Visar beställningens aktuella status.
    public OrderStatus Status { get; set; } = OrderStatus.Created;

    // Sparar datum och tid när beställningen skapas.
    public DateTime OrderTime { get; set; } = DateTime.Now;

    // Uppdaterar beställningens status.
    public void UpdateStatus(OrderStatus newStatus)
    {
        Status = newStatus;
    }

    // Lägger till en maträtt i beställningen.
    public void AddToOrder(MenuItem item)
    {
        OrderedItems.Add(item);
    }

    // Tar bort en maträtt från beställningen med hjälp av dess ID.
    public void RemoveFromOrder(int id)
    {
        var item = OrderedItems.Find(menuitem => menuitem.Id == id);

        if (item != null)
        {
            OrderedItems.Remove(item);
        }
    }

    // Beräknar det totala priset för beställningen.
    public decimal CalculateSumPrice()
    {
        return OrderedItems.Sum(item => item.Price);
    }
}
