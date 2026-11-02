
namespace RestaurangSystem.Models;

// Representerar en beställning i restaurangen.
public class Order
{
    // Ett unikt nummer för beställningen.
    public int Id { get; set; }

    // Visar beställningens aktuella status.
    public OrderStatus Status { get; set; } = OrderStatus.Created;

    // Sparar datum och tid när beställningen skapas.
    public DateTime OrderTime { get; set; } = DateTime.Now;

    // Uppdaterar beställningens status.
    public void UpdateStatus(OrderStatus newStatus)
    {
        Status = newStatus;
    }
}
