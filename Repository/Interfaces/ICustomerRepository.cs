using MiApiORACLE.DTO;
using MiApiORACLE.Models;

namespace MiApiORACLE.Repositories.IRepositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        public Task<List<CustomerDTO>> GetCustomerByIdAsync(int id);
    }
    
}