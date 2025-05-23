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
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Product)
                .Include(o => o.DeliveryStatus)
                .Include(o => o.PickupPoint)
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
                    PickupPointName = o.PickupPoint.City.Name + ", " +o.PickupPoint.Address,
                    ImageUrl = o.Product.ImageUrl,
                    PickupPoint = o.PickupPoint,
                    ConfirmationCode = o.ConfirmationCode
                    
                })
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(string id)
        {
            var order = await _context.Orders.Include(o => o.PickupPoint).FirstOrDefaultAsync(o => o.OrderID == id);
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
                .Include(o => o.PickupPoint) // Включаем ПВЗ
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
                    Size = o.Size,
                    ProductName = o.Product.Title,
                    DeliveryStatus = o.DeliveryStatus != null ? o.DeliveryStatus.StatusName : "Создан",
                    PickupPointName = o.PickupPoint.City.Name + ", " +o.PickupPoint.Address,
                    ImageUrl = o.Product.ImageUrl,
                    PickupPoint = o.PickupPoint,
                    ConfirmationCode = o.ConfirmationCode
                })
                .ToListAsync();

            return Ok(orders);
        }


        [HttpPost]
public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderRequest request)
{
    // Получаем первый статус доставки (по умолчанию)
    var firstStatus = await _context.DeliveryStatuses.FirstOrDefaultAsync();
    if (firstStatus == null)
    {
        return BadRequest("Статус доставки не найден");
    }

    // Получаем продукт по ID
    var product = await _context.Products.FindAsync(request.ProductID);
    if (product == null)
    {
        return NotFound("Продукт не найден");
    }

    // Проверка на наличие нужного размера и его количества
    var productSize = await _context.ProductSizes
        .FirstOrDefaultAsync(ps => ps.ProductID == request.ProductID && ps.Size == request.Size);

    if (productSize == null)
    {
        return NotFound("Выбранный размер не найден у данного товара");
    }

    if (productSize.Stock < request.Quantity)
    {
        return BadRequest("Недостаточно товара на складе для выбранного размера");
    }

    // Получаем пункт выдачи
    var pickupPoint = await _context.PickupPoints
        .Include(p => p.City)
        .FirstOrDefaultAsync(p => p.PickupPointId == request.PickupPointID);

    if (pickupPoint == null)
    {
        return NotFound("Пункт выдачи не найден");
    }

    // Уменьшаем количество на складе
    productSize.Stock -= request.Quantity;

    // Создаем заказ
    var newOrder = new Order
    {
        OrderID = Guid.NewGuid().ToString(),
        UserID = request.UserID,
        ProductID = request.ProductID,
        Quantity = request.Quantity,
        TotalAmount = request.TotalAmount,
        PickupPointID = request.PickupPointID,
        DeliveryStatusID = firstStatus.StatusID,
        SellerID = product.SellerID,
        OrderDate = DateTime.UtcNow,
        Size = request.Size // <-- ВАЖНО!
    };


    _context.Orders.Add(newOrder);
    await _context.SaveChangesAsync();

    // Формируем DTO
    var orderDto = new OrderDto
    {
        OrderID = newOrder.OrderID,
        UserID = newOrder.UserID,
        ProductID = newOrder.ProductID,
        Quantity = newOrder.Quantity,
        TotalAmount = newOrder.TotalAmount,
        DeliveryStatusID = newOrder.DeliveryStatusID,
        SellerID = newOrder.SellerID,
        OrderDate = newOrder.OrderDate,
        ProductName = product.Title,
        DeliveryStatus = firstStatus.StatusName,
        PickupPointName = pickupPoint.City.Name + ", " + pickupPoint.Address,
        ImageUrl = product.ImageUrl,
        PickupPoint = pickupPoint
    };

    return CreatedAtAction(nameof(GetOrder), new { id = newOrder.OrderID }, orderDto);
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

        [HttpGet("seller/{sellerId}/created-orders")]
        public async Task<IActionResult> GetCreatedProductsForSeller(string sellerId)
        {            

            var products = await _context.Orders
                .Where(o => o.SellerID == sellerId)
                .Include(o => o.Product)
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
                    PickupPointName = o.PickupPoint.City.Name + ", " +o.PickupPoint.Address,
                    ImageUrl = o.Product.ImageUrl,
                    PickupPoint = o.PickupPoint,
                    Size = o.Size,
                    ConfirmationCode = o.ConfirmationCode
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpPut("{id}/update-status")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] UpdateOrderStatusRequest request)
        {
            var order = await _context.Orders
                .Include(o => o.DeliveryStatus)
                .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            var newStatus = await _context.DeliveryStatuses.FindAsync(request.StatusID);
            if (newStatus == null)
            {
                return BadRequest("Статус с таким ID не найден");
            }

            order.DeliveryStatusID = newStatus.StatusID;
            order.DeliveryStatus = newStatus;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }


        [HttpPost("{id}/generate-confirmation-code")]
        public async Task<IActionResult> GenerateConfirmationCode(string id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            // Генерация уникального кода сдачи
            var confirmationCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            // Сохраняем код в базе данных
            order.ConfirmationCode = confirmationCode;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return Ok(new { confirmationCode });
        }

        [HttpPost("{id}/verify-confirmation-code")]
        public async Task<IActionResult> VerifyConfirmationCode(string id, [FromBody] VerifyConfirmationCodeRequest request)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderID == id);
            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            // Проверка кода
            if (order.ConfirmationCode != request.ConfirmationCode)
            {
                return BadRequest("Неверный код сдачи");
            }

            // Обновляем статус заказа на "Ожидает отправки"
            var deliveredStatus = await _context.DeliveryStatuses
                .FirstOrDefaultAsync(ds => ds.StatusName == "Ожидает отправки");

            if (deliveredStatus != null)
            {
                order.DeliveryStatusID = deliveredStatus.StatusID;
                order.DeliveryStatus = deliveredStatus;

                // Генерация нового кода подтверждения
                var newConfirmationCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
                order.ConfirmationCode = newConfirmationCode;

                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Код сдачи подтвержден. Статус заказа изменен.",
                    newConfirmationCode
                });
            }

            return BadRequest("Статус 'Ожидает отправки' не найден.");
        }

        
        [HttpPost("{id}/verify-pickup-code")]
        public async Task<IActionResult> VerifyPickupCode(string id, [FromBody] VerifyConfirmationCodeRequest request)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            // Проверка кода
            if (order.ConfirmationCode != request.ConfirmationCode)
            {
                return BadRequest("Неверный код для получения заказа");
            }

            // Обновляем статус заказа на "Завершен"
            var completedStatus = await _context.DeliveryStatuses
                .FirstOrDefaultAsync(ds => ds.StatusName == "Завершен");

            if (completedStatus != null)
            {
                order.DeliveryStatusID = completedStatus.StatusID;
                order.DeliveryStatus = completedStatus;

                // Начисляем сумму заказа на баланс пользователя
                var user = await _context.Users.FindAsync(order.SellerID);
                if (user != null)
                {
                    user.Balance += (double)order.TotalAmount -100;
                    _context.Users.Update(user);
                }

                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                return Ok("Код подтвержден. Заказ завершен, сумма начислена на баланс.");
            }

            return BadRequest("Статус 'Завершен' не найден.");
        }


        [HttpGet("seller/{sellerId}/last-month-sales")]
        public async Task<IActionResult> GetLastMonthSales(string sellerId)
        {
            // Рассчитываем дату "месяц назад" от текущего момента
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);
    
            // Получаем и группируем товары за последний месяц
            var productSales = await _context.Orders
                .Where(o => o.SellerID == sellerId && o.OrderDate >= oneMonthAgo)
                .Include(o => o.Product)
                .GroupBy(o => o.Product.Title) // Группируем по названию товара
                .Select(g => new 
                {
                    name = g.Key,          // Название товара
                    stock = g.Sum(x => x.Quantity) // Общее количество проданных единиц
                })
                .OrderByDescending(p => p.stock) // Сортируем по количеству продаж
                .ToListAsync();

            return Ok(productSales);
        }
        
        [HttpGet("seller/{sellerId}/last-six-months-sales")]
        public async Task<IActionResult> GetLastSixMonthsSales(string sellerId)
        {
            var currentDate = DateTime.UtcNow;
            var sixMonthsAgo = currentDate.AddMonths(-6);
            
            // Получаем данные о продажах за последние 6 месяцев
            var monthlySales = await _context.Orders
                .Where(o => o.SellerID == sellerId && o.OrderDate >= sixMonthsAgo)
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .ToListAsync();
        
            // Создаем результат для всех месяцев (даже если продаж не было)
            var result = new List<object>();
            var russianMonths = new string[] { 
                "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь",
                "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"
            };
        
            for (int i = 0; i < 6; i++)
            {
                var date = currentDate.AddMonths(-i);
                var monthData = monthlySales.FirstOrDefault(m => m.Year == date.Year && m.Month == date.Month);
                
                result.Add(new
                {
                    Month = russianMonths[date.Month - 1],
                    Year = date.Year,
                    TotalSold = monthData?.TotalSold ?? 0
                });
            }
        
            // Сортируем по хронологии (от старых к новым)
            result = result.OrderBy(x => 
            {
                var item = (dynamic)x;
                return new DateTime(item.Year, Array.IndexOf(russianMonths, item.Month) + 1, 1);
            }).ToList();
        
            return Ok(result);
        }


    }
}
