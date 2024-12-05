using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleShiftBackend.Data;
using StyleShiftBackend.Dto;
using StyleShiftBackend.Models;
using StyleShiftBackend.Requests;

namespace StyleShiftBackend.Controllers
{
    [Route("orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DataContext _context;

        public OrdersController(DataContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(string id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            return order;
        }
        
        [HttpGet("/get-orders-for-user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserID == userId)
                .Include(o => o.Product)
                .Include(o => o.DeliveryStatus)
                .Select(o => new OrderDto
                {
                    OrderID = o.OrderID,
                    UserID = o.UserID,
                    ProductID = o.ProductID,
                    Quantity = o.Quantity,
                    TotalAmount = o.TotalAmount,
                    DeliveryStatusID = o.DeliveryStatusID,
                    SellerID = o.SellerID,
                    OrderDate = o.OrderDate,
                    ProductName = o.Product.Title,
                    DeliveryStatus = o.DeliveryStatus != null ? o.DeliveryStatus.StatusName : "Создан",
                    DeliveryAddress = o.DeliveryAddress,
                    DeliveryCity = o.DeliveryCity,
                    DeliveryComment = o.DeliveryComment,
                    DeliveryPhone = o.DeliveryPhone,
                    ImageUrl = o.Product.ImageUrl
                })
                .ToListAsync();

            return Ok(orders);
        }

        
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(CreaeteOrderRequest request)
        {
            
            var firstStatus = await _context.DeliveryStatuses.FirstOrDefaultAsync();
            
            if (firstStatus == null)
            {
                return BadRequest("Такого статуса не существует");
            }
            
            var order = new Order
            {
                OrderID = Guid.NewGuid().ToString(),
                UserID = request.UserID,
                ProductID = request.ProductID,
                Quantity = request.Quantity,
                TotalAmount = request.TotalAmount,
                DeliveryAddress = request.DeliveryAddress,
                DeliveryComment = request.DeliveryComment,
                DeliveryPhone = request.DeliveryPhone,
                DeliveryCity = request.DeliveryCity,
                DeliveryStatusID = firstStatus.StatusID,
                SellerID = request.SellerID,
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrders), new { id = order.OrderID }, order);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
