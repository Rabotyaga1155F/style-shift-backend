using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleShiftBackend.Data;
using StyleShiftBackend.Models;

namespace StyleShiftBackend.Controllers
{
    [Route("support-request")]
    [ApiController]
    public class SupportRequestsController : ControllerBase
    {
        private readonly DataContext _context;

        public SupportRequestsController(DataContext context)
        {
            _context = context;
        }
        
        // Возвращает список заявок с их статусами
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupportRequest>>> GetSupportRequests()
        {
            return await _context.SupportRequests
                .Include(sr => sr.Status)  // Включаем статус при получении заявок
                .ToListAsync();
        }

        // Возвращает одну заявку с её статусом
        [HttpGet("{id}")]
        public async Task<ActionResult<SupportRequest>> GetSupportRequest(string id)
        {
            var supportRequest = await _context.SupportRequests
                .Include(sr => sr.Status)  // Включаем статус
                .FirstOrDefaultAsync(sr => sr.SupportRequestId == id);

            if (supportRequest == null)
            {
                return NotFound();
            }

            return supportRequest;
        }
        
        // Создание новой заявки
        [HttpPost]
        public async Task<ActionResult<SupportRequest>> PostSupportRequest(SupportRequest supportRequest)
        {
            _context.SupportRequests.Add(supportRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSupportRequest), new { id = supportRequest.SupportRequestId }, supportRequest);
        }
        
        // Обновление заявки
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSupportRequest(string id, SupportRequest supportRequest)
        {
            if (id != supportRequest.SupportRequestId)
            {
                return BadRequest();
            }

            _context.Entry(supportRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupportRequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        
        // Удаление заявки
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupportRequest(string id)
        {
            var supportRequest = await _context.SupportRequests.FindAsync(id);
            if (supportRequest == null)
            {
                return NotFound();
            }

            _context.SupportRequests.Remove(supportRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SupportRequestExists(string id)
        {
            return _context.SupportRequests.Any(e => e.SupportRequestId == id);
        }

     

        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] JsonElement body)
        {
            if (!body.TryGetProperty("newStatusId", out JsonElement newStatusIdElement) || newStatusIdElement.ValueKind != JsonValueKind.String)
            {
                return BadRequest("Неверный формат newStatusId");
            }

            string newStatusId = newStatusIdElement.GetString();

            var supportRequest = await _context.SupportRequests
                .Include(sr => sr.Status)
                .FirstOrDefaultAsync(sr => sr.SupportRequestId == id);

            if (supportRequest == null)
            {
                return NotFound();
            }

            var newStatus = await _context.SupportRequestStatuses.FindAsync(newStatusId);
            if (newStatus == null)
            {
                return NotFound("Указанный статус не найден");
            }

            // Обновляем статус
            supportRequest.StatusId = newStatusId;
            supportRequest.Status = newStatus;

            await _context.SaveChangesAsync();

            return Ok(supportRequest);
        }


    }
}
