using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiApiORACLE.Data;
using MiApiORACLE.DTO;
using MiApiORACLE.Models;
using MiApiORACLE.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace MiApiORACLE.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context) :
        base(context)
        { 
            _context = context;
        }

        public Task<List<CustomerDTO>> GetCustomerByIdAsync(int id)
        {
            var customers = _context.Customers
                .Where(c => c.CustomerId == id)
                .Select(
                    c => new CustomerDTO
                    {
                        FullName = c.FullName,
                        EmailAdress = c.EmailAdress
                    }
                )
                .ToListAsync();

            return customers;
        }
    }
}