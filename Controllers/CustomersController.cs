using System.Data.Common;
using MiApiORACLE.DTO;
using MiApiORACLE.Models;
using MiApiORACLE.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace MiApiORACLE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                return Ok(customers);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("GetCustomerById/{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }
                return Ok(customer);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetCustomerByIdDTO/{id}")]
        public async Task<IActionResult> GetCustomerByIdDTO(int id)
        {
            try
            {
                var customer = await _customerRepository.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }
                return Ok(customer);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CustomerDTO customer)
        {
            try
            {

                if (customer == null)
                {
                    return BadRequest();
                }

                var newCustomer = new Customer
                {
                    EmailAdress = customer.EmailAdress ?? string.Empty,
                    FullName = customer.FullName ?? string.Empty
                };

                await _customerRepository.AddAsync(newCustomer);
                return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerId }, customer);
            }
            catch (DbUpdateException ex) when (ex.InnerException is OracleException oraEx)
            {

                return oraEx.Number switch
                {
                    1 => Conflict("El registro ya existe."), // HTTP 409
                    2291 => BadRequest("Referencia inválida (FK)."), // HTTP 400
                    _ => StatusCode(500, "Error de base de datos.") // HTTP 500
                };
            }
            catch (OracleException ex)
            {
                return StatusCode(500, $"Error específico de Oracle: {ex.Message}");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor."); // HTTP 500
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, CustomerDTO customer)
        {
            try
            {

                if (id != customer.CustomerId)
                {
                    return BadRequest();
                }

                var existingCustomer = await _customerRepository.GetByIdAsync(id);
                if (existingCustomer == null)
                {
                    return NotFound();
                }

                existingCustomer.EmailAdress = customer.EmailAdress ?? string.Empty;
                existingCustomer.FullName = customer.FullName ?? string.Empty;

                await _customerRepository.UpdateAsync(existingCustomer);
                return NoContent();
            }
            catch (DbUpdateException ex) when (ex.InnerException is OracleException oraEx)
            {
                return oraEx.Number switch
                {
                    1 => Conflict("Violación de clave única (ORA-00001)."), // HTTP 409
                    2291 => BadRequest("Violación de clave foránea (ORA-02291)."), // HTTP 400
                    _ => StatusCode(500, "Error de base de datos.")
                };
            }
            catch (OracleException ex)
            {
                return StatusCode(500, $"Error específico de Oracle: {ex.Message}");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {

                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }

                await _customerRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (DbUpdateException ex) when (ex.InnerException is OracleException oraEx)
            {
                return oraEx.Number switch
                {
                    2292 => Conflict("No se puede eliminar el cliente porque tiene dependencias."),
                    _ => StatusCode(500, "Error al intentar eliminar en la base de datos.")
                };
            }
            catch (OracleException ex)
            {
                return StatusCode(500, $"Error específico de Oracle: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }

        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCustomers(string email)
        {
            try
            {
                var customers = await _customerRepository.FindAsync(c => c.EmailAdress.ToLower().Contains(email.ToLower()));
                if (customers == null || !customers.Any())
                {
                    return NotFound();
                }
                return Ok(customers);
            }
            catch (DbException)
            {
                // Log the exception
                return StatusCode(500, "Database error");
            }
            catch (Exception)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }
    }
}