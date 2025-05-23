using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleShiftBackend.Data;
using StyleShiftBackend.Models;

namespace StyleShiftBackend.Controllers
{
    [Route("support-request-statuses")]
    [ApiController]
    public class SupportRequestStatusesController : ControllerBase
    {
        private readonly DataContext _context;

        public SupportRequestStatusesController(DataContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupportRequestStatus>>> GetSupportRequestStatuses()
        {
            return await _context.SupportRequestStatuses.ToListAsync();
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<SupportRequestStatus>> GetSupportRequestStatus(string id)
        {
            var supportRequestStatus = await _context.SupportRequestStatuses.FindAsync(id);

            if (supportRequestStatus == null)
            {
                return NotFound();
            }

            return supportRequestStatus;
        }
        
        [HttpPost]
        public async Task<ActionResult<SupportRequestStatus>> PostSupportRequestStatus(SupportRequestStatus supportRequestStatus)
        {
            _context.SupportRequestStatuses.Add(supportRequestStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSupportRequestStatus), new { id = supportRequestStatus.SupportRequestStatusId }, supportRequestStatus);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSupportRequestStatus(string id, SupportRequestStatus supportRequestStatus)
        {
            if (id != supportRequestStatus.SupportRequestStatusId)
            {
                return BadRequest();
            }

            _context.Entry(supportRequestStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupportRequestStatusExists(id))
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
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupportRequestStatus(string id)
        {
            var supportRequestStatus = await _context.SupportRequestStatuses.FindAsync(id);
            if (supportRequestStatus == null)
            {
                return NotFound();
            }

            _context.SupportRequestStatuses.Remove(supportRequestStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SupportRequestStatusExists(string id)
        {
            return _context.SupportRequestStatuses.Any(e => e.SupportRequestStatusId == id);
        }
    }
}
