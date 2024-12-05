using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleShiftBackend.Data;
using StyleShiftBackend.Models;
using StyleShiftBackend.Requests;

namespace StyleShiftBackend.Controllers;

[ApiController]
[Route("delivery-statuses")]
public class DeliveryStatusesController : ControllerBase
{
    private readonly DataContext _context;

    public DeliveryStatusesController(DataContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeliveryStatus>>> GetDeliveryStatuses()
    {
        return await _context.DeliveryStatuses.ToListAsync();
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<DeliveryStatus>> GetDeliveryStatus(string id)
    {
        var status = await _context.DeliveryStatuses.FindAsync(id);
        if (status == null)
            return NotFound();

        return status;
    }

    [HttpPost]
    public async Task<ActionResult<DeliveryStatus>> CreateDeliveryStatus(CreateDeliveryStatusRequest request)
    {
        if (string.IsNullOrEmpty(request.StatusName))
        {
            return BadRequest("Status name is required.");
        } 

        var status = new DeliveryStatus
        {
            StatusID = Guid.NewGuid().ToString(),
            StatusName = request.StatusName
        };

        _context.DeliveryStatuses.Add(status);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDeliveryStatus), new { id = status.StatusID }, status);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDeliveryStatus(string id)
    {
        var status = await _context.DeliveryStatuses.FindAsync(id);
        if (status == null)
            return NotFound();

        _context.DeliveryStatuses.Remove(status);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
