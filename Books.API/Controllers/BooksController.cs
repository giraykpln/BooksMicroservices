#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using CORE.Models;
using Books.APP.Features.Books;

//Generated from Custom Microservices Template.
namespace Books.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<BooksController> _logger;
        private readonly IMediator _mediator;

        // Constructor: injects logger to log the errors to Kestrel Console or Output Window and mediator
        public BooksController(ILogger<BooksController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Send a query request to get query response
                var response = await _mediator.Send(new BookQueryRequest());
                // Convert the query response to a list
                var list = await response.ToListAsync();
                // If there are items, return them with 200 OK
                if (list.Any())
                    return Ok(list);
                // If no items found, return 204 No Content
                return NoContent();
            }
            catch (Exception exception)
            {
                // Log the exception
                _logger.LogError("BooksGet Exception: " + exception.Message);
                // Return 500 Internal Server Error with an error command response with message
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occured during BooksGet.")); 
            }
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                // Send a query request to get query response
                var response = await _mediator.Send(new BookQueryRequest());
                // Find the item with the given id
                var item = await response.SingleOrDefaultAsync(r => r.Id == id);
                // If item found, return it with 200 OK
                if (item is not null)
                    return Ok(item);
                // If item not found, return 204 No Content
                return NoContent();
            }
            catch (Exception exception)
            {
                // Log the exception
                _logger.LogError("BooksGetById Exception: " + exception.Message);
                // Return 500 Internal Server Error with an error command response with message
                return StatusCode(StatusCodes.Status500InternalServerError, new CommandResponse(false, "An exception occured during BooksGetById.")); 
            }
        }
	}
}
