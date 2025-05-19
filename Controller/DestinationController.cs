using Microsoft.AspNetCore.Mvc;
using FoodApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DestinationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DestinationController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("items-by-place/{place}")]
        public async Task<ActionResult<IEnumerable<string>>> GetItemsByPlace(string place)
        {
            var items = await _context.Destinations
                .Where(d => d.Place.ToLower() == place.ToLower())
                .Select(d => d.Item)
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("address-by-item/{item}")]
        public async Task<ActionResult<string>> GetAddressByItem(string item)
        {
            var result = await _context.Destinations
        .Where(d => d.Item.ToLower() == item.ToLower())
        .Select(d => new
        {
            Address = d.Address,
            Map = d.Map
        })
        .FirstOrDefaultAsync();

    if (result == null)
        return NotFound("Item not found");

    return Ok(result);
        }
    }
}
