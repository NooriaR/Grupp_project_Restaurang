namespace RestaurangSystem.Models;

// Visar vilka statusar en beställning kan ha.
public enum OrderStatus
{
    Created,
    Ongoing,
    Ready,
    Served
}