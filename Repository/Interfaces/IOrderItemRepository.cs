
using MiApiORACLE.DTO;
using MiApiORACLE.Models;

namespace MiApiORACLE.Repositories.IRepositories
{
    public interface IOrderItemRepository : IRepository<OrderItem>
    {
        Task<OrderItem> GetByOrderIdAndLineItemIdAsync(int orderId, int lineItemId);
        Task DeleteOrderItemAsync(int orderId, int lineItemId);
    }
}