using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Weather_Acknowledgement.Models;

namespace Weather_Acknowledgement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherStationController : ControllerBase
    {
        private readonly WeatherDbContext _context;
        private readonly ILogger<WeatherStationController> _logger;

        public WeatherStationController(
            WeatherDbContext context,
            ILogger<WeatherStationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherStation>>> GetWeatherStations()
        {
            try
            {
                var stations = await _context.WeatherStations
                    .Include(w => w.Readings)
                    .ToListAsync();
                return Ok(stations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving weather stations");
                return StatusCode(500, "Internal server error while retrieving weather stations");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WeatherStation>> GetWeatherStation(int id)
        {
            try
            {
                var station = await _context.WeatherStations
                    .Include(w => w.Readings)
                    .FirstOrDefaultAsync(w => w.Id == id);

                if (station == null)
                {
                    return NotFound($"Weather station with ID {id} not found");
                }

                return Ok(station);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving weather station {Id}", id);
                return StatusCode(500, "Internal server error while retrieving weather station");
            }
        }

        [HttpPost]
        public async Task<ActionResult<WeatherStation>> CreateWeatherStation(WeatherStation station)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                station.InstallationDate = DateTime.UtcNow;
                _context.WeatherStations.Add(station);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetWeatherStation),
                    new { id = station.Id },
                    station);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating weather station");
                return StatusCode(500, "Internal server error while creating weather station");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeatherStation(int id, WeatherStation station)
        {
            if (id != station.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var existingStation = await _context.WeatherStations.FindAsync(id);
                if (existingStation == null)
                {
                    return NotFound($"Weather station with ID {id} not found");
                }

                existingStation.Name = station.Name;
                existingStation.Location = station.Location;
                existingStation.Latitude = station.Latitude;
                existingStation.Longitude = station.Longitude;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating weather station {Id}", id);
                return StatusCode(409, "Concurrency error while updating weather station");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating weather station {Id}", id);
                return StatusCode(500, "Internal server error while updating weather station");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeatherStation(int id)
        {
            try
            {
                var station = await _context.WeatherStations.FindAsync(id);
                if (station == null)
                {
                    return NotFound($"Weather station with ID {id} not found");
                }

                _context.WeatherStations.Remove(station);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting weather station {Id}", id);
                return StatusCode(500, "Internal server error while deleting weather station");
            }
        }
    }
}
