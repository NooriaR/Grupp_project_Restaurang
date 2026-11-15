
using RestaurangSystem.Models;
using Xunit;

// Testar metoderna i Order-klassen.
public class OrderTests
{
    // Kontrollerar att summan av två maträtter blir rätt.
    [Fact]
    public void CalculateSumPrice_TwoItems_ReturnsCorrectTotal()
    {
        // Arrange
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

        // Act
        decimal total = order.CalculateSumPrice();

        // Assert
        Assert.Equal(270m, total);
    }

    // Kontrollerar att en maträtt läggs till i beställningen.
    [Fact]
    public void AddToOrder_AddsItem()
    {
        // Arrange
        Order order = new Order();
        MenuItem pizza = new MenuItem
        {
            Id = 1,
            Name = "Pizza",
            Price = 120m
        };

        // Act
        order.AddToOrder(pizza);

        // Assert
        Assert.Single(order.OrderedItems);
        Assert.Contains(pizza, order.OrderedItems);
    }

    // Kontrollerar att rätt maträtt tas bort från beställningen.
    [Fact]
    public void RemoveFromOrder_RemovesCorrectItem()
    {
        // Arrange
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

        // Act
        order.RemoveFromOrder(1);

        // Assert
        Assert.Single(order.OrderedItems);
        Assert.DoesNotContain(pizza, order.OrderedItems);
        Assert.Contains(pasta, order.OrderedItems);
    }

    // Kontrollerar att inget tas bort om ID inte finns.
    [Fact]
    public void RemoveFromOrder_InvalidId_DoesNotRemoveItem()
    {
        // Arrange
        Order order = new Order();

        MenuItem pizza = new MenuItem
        {
            Id = 1,
            Name = "Pizza",
            Price = 120m
        };

        order.AddToOrder(pizza);

        // Act
        order.RemoveFromOrder(99);

        // Assert
        Assert.Single(order.OrderedItems);
        Assert.Contains(pizza, order.OrderedItems);
    }

    // Kontrollerar att beställningens status uppdateras.
    [Fact]
    public void UpdateStatus_ChangesStatus()
    {
        // Arrange
        Order order = new Order();

        // Act
        order.UpdateStatus(OrderStatus.Ongoing);

        // Assert
        Assert.Equal(OrderStatus.Ongoing, order.Status);
    }

    // Kontrollerar att en tom beställning har totalpriset noll.
    [Fact]
    public void CalculateSumPrice_EmptyOrder_ReturnsZero()
    {
        // Arrange
        Order order = new Order();

        // Act
        decimal total = order.CalculateSumPrice();

        // Assert
        Assert.Equal(0m, total);
    }
}
