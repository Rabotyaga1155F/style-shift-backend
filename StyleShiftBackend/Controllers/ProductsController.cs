using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StyleShiftBackend.Data;
using StyleShiftBackend.Models;
using StyleShiftBackend.Requests;
using StyleShiftBackend.Services.YandexStorage;

[Route("products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly DataContext _context;

    public ProductsController(DataContext context)
    {
        _context = context;
    }
    
    private int GetSizeOrder(string size)
    {
        var order = new List<string> { "XS", "S", "M", "L", "XL", "XXL", "XXXL" };
        var index = order.IndexOf(size.ToUpper());
        return index >= 0 ? index : int.MaxValue;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetProducts()
    {
        var products = await _context.Products
            .Include(p => p.Seller)
            .Include(p => p.Category)
            .Include(p => p.Sizes)
            .Where(p => p.Seller.Verification == true && !p.IsDeleted)
            .OrderByDescending(p => p.Views)
            .ToListAsync();

        var result = products.Select(p => new
        {
            p.ProductID,
            p.SellerID,
            SellerName = p.Seller.UserName,
            p.CategoryID,
            CategoryName = p.Category.CategoryName,
            p.Title,
            p.Description,
            p.Price,
            p.ImageUrl,
            Sizes = p.Sizes
                .OrderBy(s => IsNumericSize(s.Size))               // сначала буквенные, потом числовые
                .ThenBy(s => GetSizeSortKey(s.Size))               // сортировка по значению
                .Select(s => new
                {
                    s.Size,
                    s.Stock
                })
                .ToList()
        });

        return Ok(result);
    }
    

    private bool IsNumericSize(string size)
    {
        return int.TryParse(size, out _);
    }

    private int GetSizeSortKey(string size)
    {
        if (int.TryParse(size, out var numeric)) return numeric;

        var order = new List<string> { "XS", "S", "M", "L", "XL", "XXL", "XXXL" };
        var index = order.IndexOf(size.ToUpper());
        return index >= 0 ? index : int.MaxValue;
    }


    [HttpGet("seller/{sellerId}")]
    public async Task<ActionResult<IEnumerable<object>>> GetProductsBySeller(string sellerId)
    {
        var products = await _context.Products
            .Where(p => p.SellerID == sellerId && p.Seller.Verification && !p.IsDeleted)
            .Include(p => p.Seller)
            .Include(p => p.Category)
            .Include(p => p.Sizes)
            .ToListAsync();

        var result = products.Select(p => new
        {
            p.ProductID,
            p.SellerID,
            SellerName = p.Seller.UserName,
            p.CategoryID,
            CategoryName = p.Category.CategoryName,
            p.Title,
            p.Description,
            p.Price,
            p.ImageUrl,
            Sizes = p.Sizes
                .OrderBy(s => IsNumericSize(s.Size))
                .ThenBy(s => GetSizeSortKey(s.Size))
                .Select(s => new
                {
                    s.Size,
                    s.Stock
                })
                .ToList()
        });

        return Ok(result);
    }



    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(string id)
    {
        var product = await _context.Products
            .Include(p => p.Seller)
            .Include(p => p.Category)
            .Include(p => p.Sizes) // Добавляем размеры
            .Where(p => p.ProductID == id && p.Seller.Verification == true && !p.IsDeleted)
            .Select(p => new
            {
                p.ProductID,
                p.SellerID,
                SellerName = p.Seller.UserName,
                p.CategoryID,
                CategoryName = p.Category.CategoryName,
                p.Title,
                p.Description,
                p.Price,
                p.ImageUrl,
                Sizes = p.Sizes
                    .OrderBy(s => IsNumericSize(s.Size))               // сначала буквенные, потом числовые
                    .ThenBy(s => GetSizeSortKey(s.Size))               // сортировка по значению
                    .Select(s => new
                    {
                        s.Size,
                        s.Stock
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (product == null)
            return NotFound();

        return Ok(product);
    }
    
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct([FromForm] CreateProductRequest request, [FromServices] YandexStorageService storageService)
    {
        string imageUrl = null;

        if (request.Image != null && request.Image.Length > 0)
        {
            imageUrl = await storageService.UploadFileAsync(
                request.Image.OpenReadStream(),
                request.Image.FileName,
                request.Image.ContentType
            );
        }

        var product = new Product
        {
            ProductID = Guid.NewGuid().ToString(),
            SellerID = request.SellerID,
            CategoryID = request.CategoryID,
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = imageUrl
        };

        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                var sizes = JsonConvert.DeserializeObject<List<ProductSizeRequest>>(request.SizesJson);

                foreach (var size in sizes)
                {
                    var productSize = new ProductSize
                    {
                        ProductID = product.ProductID,
                        Size = size.Size,
                        Stock = size.Stock
                    };

                    _context.ProductSizes.Add(productSize);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetProduct), new { id = product.ProductID }, product);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Ошибка при добавлении товара и размеров.");
            }
        }
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(string id, [FromForm] CreateProductRequest request, [FromServices] YandexStorageService storageService)
    {
        var product = await _context.Products
            .Include(p => p.Sizes)
            .FirstOrDefaultAsync(p => p.ProductID == id);

        if (product == null)
            return NotFound();

        product.Title = request.Title;
        product.Description = request.Description ?? product.Description;
        product.Price = request.Price;
        product.CategoryID = request.CategoryID;
        product.SellerID = request.SellerID;

        if (request.Image != null && request.Image.Length > 0)
        {
            product.ImageUrl = await storageService.UploadFileAsync(
                request.Image.OpenReadStream(),
                request.Image.FileName,
                request.Image.ContentType
            );
        }

        _context.Products.Update(product);

        var sizes = JsonConvert.DeserializeObject<List<ProductSizeRequest>>(request.SizesJson);

        foreach (var size in sizes)
        {
            var existing = await _context.ProductSizes
                .FirstOrDefaultAsync(s => s.ProductID == id && s.Size == size.Size);

            if (existing != null)
            {
                existing.Stock = size.Stock;
            }
            else
            {
                _context.ProductSizes.Add(new ProductSize
                {
                    ProductID = id,
                    Size = size.Size,
                    Stock = size.Stock
                });
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(string id, [FromServices] YandexStorageService storageService)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        product.IsDeleted = true;

        if (!string.IsNullOrEmpty(product.ImageUrl))
        {
            var key = product.ImageUrl.Split($"{product.ProductID}/").Last();
            await storageService.DeleteFileAsync($"products/{product.ProductID}/{key}");
        }

        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpPost("increment-views/{id}")]
    public async Task<IActionResult> IncrementViews(string id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        product.Views += 1;
        await _context.SaveChangesAsync();

        return NoContent();
    }

}
