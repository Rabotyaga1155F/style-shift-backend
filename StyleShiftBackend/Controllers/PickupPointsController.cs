using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleShiftBackend.Data;
using StyleShiftBackend.Dto;
using StyleShiftBackend.Models;

namespace StyleShiftBackend.Controllers
{
    [Route("pickup-points")]
    [ApiController]
    public class PickupPointsController : ControllerBase
    {
        private readonly DataContext _context;

        public PickupPointsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PickupPoint>>> GetPickupPoints()
        {
            return await _context.PickupPoints.Include(p => p.City).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PickupPoint>> GetPickupPoint(string id)
        {
            var pickupPoint = await _context.PickupPoints.Include(p => p.City)
                                                          .FirstOrDefaultAsync(p => p.PickupPointId == id);

            if (pickupPoint == null)
            {
                return NotFound();
            }

            return pickupPoint;
        }

        [HttpPost]
        public async Task<ActionResult<PickupPoint>> PostPickupPoint(PickupPointDto pickupPointDto)
        {
            var pickupPoint = new PickupPoint
            {
                Address = pickupPointDto.Address,
                CityId = pickupPointDto.CityId,
                Latitude = pickupPointDto.Latitude,
                Longitude = pickupPointDto.Longitude
            };

            _context.PickupPoints.Add(pickupPoint);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPickupPoint", new { id = pickupPoint.PickupPointId }, pickupPoint);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPickupPoint(string id, PickupPointDto pickupPointDto)
        {
            var pickupPoint = await _context.PickupPoints.FindAsync(id);
            if (pickupPoint == null)
            {
                return NotFound();
            }

            pickupPoint.Address = pickupPointDto.Address;
            pickupPoint.CityId = pickupPointDto.CityId;
            pickupPoint.Latitude = pickupPointDto.Latitude;
            pickupPoint.Longitude = pickupPointDto.Longitude;

            _context.Entry(pickupPoint).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PickupPointExists(id))
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
        public async Task<IActionResult> DeletePickupPoint(string id)
        {
            var pickupPoint = await _context.PickupPoints.FindAsync(id);
            if (pickupPoint == null)
            {
                return NotFound();
            }

            _context.PickupPoints.Remove(pickupPoint);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("city/{cityId}")]
        public async Task<ActionResult<IEnumerable<PickupPoint>>> GetPickupPointsByCity(string cityId)
        {
            var pickupPoints = await _context.PickupPoints
                                              .Where(p => p.CityId == cityId)
                                              .Include(p => p.City)
                                              .ToListAsync();

            if (pickupPoints == null || !pickupPoints.Any())
            {
                return NotFound(new { message = "Пункты выдачи заказов для данного города не найдены." });
            }

            return Ok(pickupPoints);
        }

        private bool PickupPointExists(string id)
        {
            return _context.PickupPoints.Any(e => e.PickupPointId == id);
        }
    }
}
