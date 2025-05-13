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
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemRepository _orderItemRepository;
        public OrderItemController(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrderItems()
        {
            try
            {
                var orderItems = await _orderItemRepository.GetAllAsync();
                return Ok(orderItems);
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

        [HttpGet("{orderId}/{lineItemId}")]
        public async Task<IActionResult> GetOrderItemById(int orderId, int lineItemId)
        {
            try
            {

                var orderItem = await _orderItemRepository.GetByOrderIdAndLineItemIdAsync(orderId, lineItemId);
                if (orderItem == null)
                {
                    return NotFound();
                }
                return Ok(orderItem);
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
        public async Task<IActionResult> CreateOrderItem(OrderItemDTO orderItem)
        {
            try
            {
                if (orderItem == null)
                {
                    return BadRequest();
                }

                var newOrderItem = new OrderItem
                {
                    OrderId = orderItem.OrderId,
                    LineItemId = orderItem.LineItemId,
                    ProductId = orderItem.ProductId,
                    UnitPrice = orderItem.UnitPrice,
                    Quantity = orderItem.Quantity,
                    ShipmentId = 1

                };

                await _orderItemRepository.AddAsync(newOrderItem);
                return CreatedAtAction(nameof(GetOrderItemById), new { orderId = newOrderItem.OrderId, LineItemId = newOrderItem.LineItemId }, newOrderItem);
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

        [HttpPut("{orderId}/{lineItemId}")]
        public async Task<IActionResult> UpdateOrderItem(int orderId, int lineItemId, OrderItemDTO orderItem)
        {
            try
            {
                if (orderId != orderItem.OrderId || lineItemId != orderItem.LineItemId)
                {
                    return BadRequest();
                }

                var existingOrderItem = await _orderItemRepository.GetByOrderIdAndLineItemIdAsync(orderItem.OrderId, orderItem.LineItemId);
                if (existingOrderItem == null)
                {
                    return NotFound();
                }

                existingOrderItem.ProductId = orderItem.ProductId;
                existingOrderItem.UnitPrice = orderItem.UnitPrice;
                existingOrderItem.Quantity = orderItem.Quantity;

                await _orderItemRepository.UpdateAsync(existingOrderItem);
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

        [HttpDelete("{orderId}/{lineItemId}")]
        public async Task<IActionResult> DeleteOrderItem(int orderId, int lineItemId)
        {
            try
            {
                var orderItem = await _orderItemRepository.GetByOrderIdAndLineItemIdAsync(orderId, lineItemId);
                if (orderItem == null)
                {
                    return NotFound();
                }

                await _orderItemRepository.DeleteOrderItemAsync(orderId, lineItemId);
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

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetOrderItemsByOrderId(int orderId)
        {
            try
            {

                var orderItems = await _orderItemRepository.FindAsync(x => x.OrderId == orderId);
                if (orderItems == null || !orderItems.Any())
                {
                    return NotFound();
                }
                return Ok(orderItems);
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

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetOrderItemsByProductId(int productId)
        {
            try
            {

                var orderItems = await _orderItemRepository.FindAsync(x => x.ProductId == productId);
                if (orderItems == null || !orderItems.Any())
                {
                    return NotFound();
                }
                return Ok(orderItems);
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