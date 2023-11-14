using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shopping.Data;

namespace Shopping.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly DataContext _context;

        public CountriesController(DataContext context)
        {
           _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            return Ok(await _context.Countries.Include(x => x.States).ThenInclude(x => x.Cities).ToListAsync());
        }
    }
}
