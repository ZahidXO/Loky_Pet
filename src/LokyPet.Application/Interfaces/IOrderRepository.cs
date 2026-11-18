namespace LokyPet.Application.Interfaces;
using LokyPet.Domain.Entities;

public interface IOrderRepository
{
    Task<Order> CreateOrderAsync(Order order);
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<Order?> GetOrderByNumberAsync(string orderNumber);
    Task<List<Order>> GetOrdersByUserAsync(Guid userId);
    Task<List<Order>> GetAllOrdersAsync();
    Task UpdateOrderStatusAsync(Guid orderId, string status, string? trackingNumber = null);
}
