using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleShiftBackend.Data;
using StyleShiftBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StyleShiftBackend.Controllers
{
    [Route("style-card-statuses")]
    [ApiController]
    public class StyleCardStatusesController : ControllerBase
    {
        private readonly DataContext _context;

        public StyleCardStatusesController(DataContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StyleCardStatuses>>> GetStyleCardStatuses()
        {
            var statuses = await _context.StyleCardStatuses.ToListAsync();

            if (!statuses.Any())
            {
                return NotFound("Статусы не найдены.");
            }

            return Ok(statuses);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<StyleCardStatuses>> GetStyleCardStatusById(string id)
        {
            var status = await _context.StyleCardStatuses.FindAsync(id);

            if (status == null)
            {
                return NotFound("Статус не найден.");
            }

            return Ok(status);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateStyleCardStatus([FromBody] StyleCardStatuses styleCardStatus)
        {
            if (styleCardStatus == null)
            {
                return BadRequest("Статус карточки не может быть пустым.");
            }

            _context.StyleCardStatuses.Add(styleCardStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStyleCardStatusById), new { id = styleCardStatus.ID }, styleCardStatus);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStyleCardStatus(string id, [FromBody] StyleCardStatuses updatedStatus)
        {
            if (id != updatedStatus.ID)
            {
                return BadRequest("Id статуса не совпадает.");
            }

            var status = await _context.StyleCardStatuses.FindAsync(id);

            if (status == null)
            {
                return NotFound("Статус не найден.");
            }

            status.Name = updatedStatus.Name;

            _context.Entry(status).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStyleCardStatus(string id)
        {
            var status = await _context.StyleCardStatuses.FindAsync(id);

            if (status == null)
            {
                return NotFound("Статус не найден.");
            }

            _context.StyleCardStatuses.Remove(status);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
