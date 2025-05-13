using MiApiORACLE.Data;
using MiApiORACLE.Models;
using MiApiORACLE.Repositories.IRepositories;

namespace MiApiORACLE.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }
        
    }
}