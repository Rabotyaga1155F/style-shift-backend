using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleShiftBackend.Data;
using StyleShiftBackend.Models;
using System.Threading.Tasks;

namespace StyleShiftBackend.Controllers
{
    [Route("style-cards")]
    [ApiController]
    public class StyleCardController : ControllerBase
    {
        private readonly DataContext _context;

        public StyleCardController(DataContext context)
        {
            _context = context;
        }
        
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StyleCard>>> GetAllStyleCards()
        {
            var styleCards = await _context.StyleCards
                .Include(sc => sc.Status)
                .Include(sc => sc.User)
                .ToListAsync();

            return Ok(styleCards);
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateStyleCard([FromBody] StyleCard styleCard)
        {
            if (styleCard == null)
            {
                return BadRequest("Данные карточки стиля не могут быть пустыми.");
            }

            // Проверка на существование User
            var userExists = await _context.Users.AnyAsync(u => u.Id == styleCard.UserID);
            if (!userExists)
            {
                return BadRequest("Пользователь с указанным ID не существует.");
            }

            // Проверка на существование Status
            var statusExists = await _context.StyleCardStatuses.AnyAsync(s => s.ID == styleCard.StyleCardStatusID);
            if (!statusExists)
            {
                return BadRequest("Статус с указанным ID не существует.");
            }
            
            _context.StyleCards.Add(styleCard);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStyleCardById), new { id = styleCard.StyleCardID }, styleCard);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<StyleCard>> GetStyleCardById(string id)
        {
            var styleCard = await _context.StyleCards
                .Include(sc => sc.Status)
                .Include(sc => sc.User)
                .FirstOrDefaultAsync(sc => sc.StyleCardID == id);

            if (styleCard == null)
            {
                return NotFound("Карточка стиля не найдена.");
            }

            return Ok(styleCard);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStyleCard(string id)
        {
            var styleCard = await _context.StyleCards.FindAsync(id);

            if (styleCard == null)
            {
                return NotFound("Карточка стиля не найдена.");
            }

            _context.StyleCards.Remove(styleCard);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStyleCard(string id, [FromBody] StyleCard updatedStyleCard)
        {
            if (id != updatedStyleCard.StyleCardID)
            {
                return BadRequest("Id карточки стиля не совпадает.");
            }

            var styleCard = await _context.StyleCards.FindAsync(id);

            if (styleCard == null)
            {
                return NotFound("Карточка стиля не найдена.");
            }
            
            styleCard.Quiz = updatedStyleCard.Quiz;
            styleCard.StyleCardStatusID = updatedStyleCard.StyleCardStatusID;
            styleCard.UserID = updatedStyleCard.UserID;

            _context.Entry(styleCard).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStyleCardStatus(string id, [FromBody] StatusUpdateRequest request)
        {
            var styleCard = await _context.StyleCards.FindAsync(id);

            if (styleCard == null)
            {
                return NotFound("Карточка стиля не найдена.");
            }

            var status = await _context.StyleCardStatuses.FindAsync(request.StatusId);
            if (status == null)
            {
                return BadRequest("Статус не найден.");
            }

            styleCard.StyleCardStatusID = request.StatusId;

            _context.Entry(styleCard).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public class StatusUpdateRequest
        {
            public string StatusId { get; set; }
        }

        
        
        
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<StyleCard>>> GetStyleCardByUserId(string userId)
        {
            var styleCards = await _context.StyleCards
                .Include(sc => sc.Status)
                .Include(sc => sc.User)
                .Where(sc => sc.UserID == userId)
                .ToListAsync();
            
            return Ok(styleCards);
        }
    }
}
