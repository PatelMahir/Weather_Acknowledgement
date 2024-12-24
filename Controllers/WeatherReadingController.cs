using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Weather_Acknowledgement.Models;

namespace Weather_Acknowledgement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherReadingController : ControllerBase
    {
        private readonly WeatherDbContext _context;
        private readonly ILogger<WeatherReadingController> _logger;

        public WeatherReadingController(
            WeatherDbContext context,
            ILogger<WeatherReadingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherReading>>> GetWeatherReadings(
            [FromQuery] int? stationId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                var query = _context.WeatherReadings.AsQueryable();

                if (stationId.HasValue)
                {
                    query = query.Where(r => r.WeatherStationId == stationId.Value);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(r => r.Timestamp >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(r => r.Timestamp <= toDate.Value);
                }

                var readings = await query
                    .Include(r => r.WeatherStationId)
                    .OrderByDescending(r => r.Timestamp)
                    .ToListAsync();

                return Ok(readings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving weather readings");
                return StatusCode(500, "Internal server error while retrieving weather readings");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WeatherReading>> GetWeatherReading(int id)
        {
            try
            {
                var reading = await _context.WeatherReadings
                    .Include(r => r.WeatherStationId)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reading == null)
                {
                    return NotFound($"Weather reading with ID {id} not found");
                }

                return Ok(reading);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving weather reading {Id}", id);
                return StatusCode(500, "Internal server error while retrieving weather reading");
            }
        }

        [HttpPost]
        public async Task<ActionResult<WeatherReading>> CreateWeatherReading(WeatherReading reading)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verify the station exists
                var stationExists = await _context.WeatherStations
                    .AnyAsync(s => s.Id == reading.WeatherStationId);

                if (!stationExists)
                {
                    return BadRequest($"Weather station with ID {reading.WeatherStationId} does not exist");
                }

                reading.Timestamp = DateTime.UtcNow;
                _context.WeatherReadings.Add(reading);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetWeatherReading),
                    new { id = reading.Id },
                    reading);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating weather reading");
                return StatusCode(500, "Internal server error while creating weather reading");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeatherReading(int id, WeatherReading reading)
        {
            if (id != reading.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var existingReading = await _context.WeatherReadings.FindAsync(id);
                if (existingReading == null)
                {
                    return NotFound($"Weather reading with ID {id} not found");
                }

                existingReading.Temperature = reading.Temperature;
                existingReading.Humidity = reading.Humidity;
                existingReading.WindSpeed = reading.WindSpeed;
                existingReading.WindDirection = reading.WindDirection;
                existingReading.Precipitation = reading.Precipitation;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating weather reading {Id}", id);
                return StatusCode(409, "Concurrency error while updating weather reading");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating weather reading {Id}", id);
                return StatusCode(500, "Internal server error while updating weather reading");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeatherReading(int id)
        {
            try
            {
                var reading = await _context.WeatherReadings.FindAsync(id);
                if (reading == null)
                {
                    return NotFound($"Weather reading with ID {id} not found");
                }

                _context.WeatherReadings.Remove(reading);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting weather reading {Id}", id);
                return StatusCode(500, "Internal server error while deleting weather reading");
            }
        }

        [HttpGet("stats/{stationId}")]
        public async Task<ActionResult> GetWeatherStats(int stationId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            try
            {
                var query = _context.WeatherReadings
                    .Where(r => r.WeatherStationId == stationId);

                if (fromDate.HasValue)
                    query = query.Where(r => r.Timestamp >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(r => r.Timestamp <= toDate.Value);

                var stats = await query.GroupBy(r => r.WeatherStationId)
                    .Select(g => new
                    {
                        StationId = g.Key,
                        ReadingsCount = g.Count(),
                        AverageTemperature = g.Average(r => r.Temperature),
                        AverageHumidity = g.Average(r => r.Humidity),
                        AverageWindSpeed = g.Average(r => r.WindSpeed),
                        TotalPrecipitation = g.Sum(r => r.Precipitation),
                        MinTemperature = g.Min(r => r.Temperature),
                        MaxTemperature = g.Max(r => r.Temperature)
                    })
                    .FirstOrDefaultAsync();

                if (stats == null)
                {
                    return NotFound($"No readings found for station ID {stationId}");
                }

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving weather stats for station {StationId}", stationId);
                return StatusCode(500, "Internal server error while retrieving weather stats");
            }
        }
    }
}
