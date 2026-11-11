
using RestaurangSystem.Models;
using Xunit;

// Testar att Order-klassen räknar rätt.
public class OrderTests
{
    // Kontrollerar att summan av två maträtter blir rätt.
    [Fact]
    public void CalculateSumPrice_TwoItems_ReturnsCorrectTotal()
    {
        // Arrange - Skapar en beställning och två maträtter.
        Order order = new Order();

        MenuItem pizza = new MenuItem
        {
            Id = 1,
            Name = "Pizza",
            Price = 120m
        };

        MenuItem pasta = new MenuItem
        {
            Id = 2,
            Name = "Pasta",
            Price = 150m
        };

        order.AddToOrder(pizza);
        order.AddToOrder(pasta);

        // Act - Beräknar det totala priset.
        decimal total = order.CalculateSumPrice();

        // Assert - Kontrollerar att summan är 270 kr.
        Assert.Equal(270m, total);
    }
}
