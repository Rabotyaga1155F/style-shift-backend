using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleShiftBackend.Data;
using StyleShiftBackend.Models;
using StyleShiftBackend.Requests;

[Route("products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly DataContext _context;

    public ProductsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var orderedProductIds = await _context.Orders
            .Select(o => o.ProductID)
            .ToListAsync();

        var products = await _context.Products
            .Include(p => p.Seller)
            .Include(p => p.Category)
            .Where(p => !orderedProductIds.Contains(p.ProductID))
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
                p.ImageUrl
            })
            .ToListAsync();

        return Ok(products);
    }
    
    [HttpGet("seller/{sellerId}")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsBySeller(string sellerId)
    {
        
        var orderedProductIds = await _context.Orders
            .Select(o => o.ProductID)
            .ToListAsync();
        
        var products = await _context.Products
            .Where(p => p.SellerID == sellerId)
            .Include(p => p.Seller)
            .Include(p => p.Category)
            .Where(p => !orderedProductIds.Contains(p.ProductID))
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
                p.ImageUrl
            })
            .ToListAsync();

        if (!products.Any())
            return NotFound();

        return Ok(products);
    }



    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(string id)
    {
        var product = await _context.Products
            .Include(p => p.Seller)
            .Include(p => p.Category)
            .Where(p => p.ProductID == id)
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
                p.ImageUrl
            })
            .FirstOrDefaultAsync();

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(CreateProductRequest request)
    {
        var product = new Product
        {
            ProductID = Guid.NewGuid().ToString(),
            SellerID = request.SellerID,
            CategoryID = request.CategoryID,
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            ImageUrl = request.ImageUrl
            
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.ProductID }, product);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(string id, CreateProductRequest request)
    {
        var product = await _context.Products
            .Include(p => p.Seller)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductID == id);

        if (product == null)
            return NotFound();
        
        product.Title = request.Title;
        product.Description = request.Description ?? product.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.ImageUrl = request.ImageUrl;
        product.CategoryID = request.CategoryID;
        product.SellerID = request.SellerID;
        
        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}