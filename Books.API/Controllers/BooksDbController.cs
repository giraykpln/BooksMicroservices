using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Books.APP.Domain;

namespace Books.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksDbController : ControllerBase
    {
        [HttpGet, Route("~/api/SeedDb")] // route for the get action
                                         // changed with the Route
                                         // attribute: api/SeedDb
        public IActionResult Seed()
        {
            // Initialize the BooksDbFactory instance.
            var dbFactory = new BooksDbFactory();
            // Seed the database using the SeedDb method.
            dbFactory.SeedDb();
            // Return HTTP OK status result (200) with a success message.
            return Ok("Database seed successful.");
        }

    }
}
