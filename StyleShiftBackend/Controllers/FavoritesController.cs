using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleShiftBackend.Models;
using System.Threading.Tasks;
using System.Linq;
using StyleShiftBackend.Data;
using StyleShiftBackend.Requests;

namespace StyleShiftBackend.Controllers
{
    [Route("favorites")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly DataContext _context;

        public FavoritesController(DataContext context)
        {
            _context = context;
        }
        
        
        
        [HttpGet("{id}")]
        public async Task<ActionResult> GetFavoritesByUserId(string id)
        {
            var products = await _context.Favorites
                .Where(f => f.UserID == id)
                .Include(f => f.Product) 
                .Select(f => f.Product) 
                .ToListAsync();
            
            return Ok(products);
        }
        
        [HttpPost]
        public async Task<ActionResult> AddToFavorites([FromBody] CreateFavoriteRequest request)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserID);
            if (!userExists)
            {
                return BadRequest($"Пользователя с id {request.UserID} не существует.");
            }
            
            var productExists = await _context.Products.AnyAsync(p => p.ProductID == request.ProductID);
            if (!productExists)
            {
                return BadRequest($"Товара с id {request.ProductID} не существует.");
            }
            
            var existingFavorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserID == request.UserID && f.ProductID == request.ProductID);

            if (existingFavorite != null)
            {
                return Conflict("Товар уже добавлен в избранное.");
            }
            
            var favorite = new Favorite
            {
                FavoriteID = Guid.NewGuid().ToString(),
                UserID = request.UserID,
                ProductID = request.ProductID
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFavoritesByUserId), new { id = request.UserID }, favorite);
        }
        
        [HttpDelete]
        public async Task<ActionResult> RemoveFromFavorites([FromBody] CreateFavoriteRequest request)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserID == request.UserID && f.ProductID == request.ProductID);

            if (favorite == null)
            {
                return NotFound("Товар не найден в избранном.");
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
