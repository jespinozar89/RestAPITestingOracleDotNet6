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
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderRepository.GetAllAsync();
                return Ok(orders);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {

                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                {
                    return NotFound();
                }
                return Ok(order);
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
        public async Task<IActionResult> CreateOrder(OrderDTO order)
        {
            try
            {
                if (order == null)
                {
                    return BadRequest();
                }

                var newOrder = new Order
                {
                    CustomerId = order.CustomerId,
                    OrderStatus = order.OrderStatus,
                    OrderTimestamp = DateTime.UtcNow,
                    StoreId = 1
                };

                await _orderRepository.AddAsync(newOrder);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
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
        public async Task<IActionResult> UpdateOrder(int id, OrderDTO order)
        {
            try
            {

                if (id != order.OrderId)
                {
                    return BadRequest();
                }

                var existingOrder = await _orderRepository.GetByIdAsync(id);
                if (existingOrder == null)
                {
                    return NotFound();
                }

                existingOrder.CustomerId = order.CustomerId;
                existingOrder.OrderStatus = order.OrderStatus;
                existingOrder.OrderTimestamp = DateTime.UtcNow;

                await _orderRepository.UpdateAsync(existingOrder);
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
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {

                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                await _orderRepository.DeleteAsync(id);
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

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetOrdersByCustomerId(int customerId)
        {
            try
            {
                var orders = await _orderRepository.FindAsync(o => o.CustomerId == customerId);
                if (orders == null || !orders.Any())
                {
                    return NotFound();
                }
                return Ok(orders);
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

        [HttpGet("store/{orderTimestamp}")]
        public async Task<IActionResult> GetOrdersByOrderTimestamp(DateTime orderTimestamp)
        {
            try
            {

                var orders = await _orderRepository.FindAsync(o => o.OrderTimestamp.Date == orderTimestamp.Date);
                if (orders == null || !orders.Any())
                {
                    return NotFound();
                }
                return Ok(orders);
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