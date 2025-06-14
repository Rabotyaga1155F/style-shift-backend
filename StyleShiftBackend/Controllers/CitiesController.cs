using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleShiftBackend.Data;
using StyleShiftBackend.Dto;
using StyleShiftBackend.Models;

namespace StyleShiftBackend.Controllers
{
    [Route("cities")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly DataContext _context;

        public CitiesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            return await _context.Cities.Include(c => c.PickupPoints).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(string id)
        {
            var city = await _context.Cities.Include(c => c.PickupPoints)
                                             .FirstOrDefaultAsync(c => c.CityId == id);

            if (city == null)
            {
                return NotFound();
            }

            return city;
        }

        [HttpPost]
        public async Task<ActionResult<City>> PostCity(CityDto cityDto)
        {
            var city = new City
            {
                Name = cityDto.Name
            };

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { id = city.CityId }, city);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(string id, CityDto cityDto)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            // Обновляем только поле Name
            city.Name = cityDto.Name;

            _context.Entry(city).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(string id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(string id)
        {
            return _context.Cities.Any(e => e.CityId == id);
        }
    }
}
