using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice2.Context;
using Practice2.Models;

namespace Practice2.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class BookController : Controller
    {
        private readonly BookContext _context;

        public BookController(BookContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Read()
        {
            var list = await _context.Books.ToListAsync();
            return Ok(list);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var updatedList = await _context.Books.ToListAsync();
            return Ok(updatedList);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Book updatedBook)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
                return BadRequest("Can't find requested index.");

            existingBook.Title = updatedBook.Title;
            existingBook.Author = updatedBook.Author;
            existingBook.PublishYear = updatedBook.PublishYear;
            existingBook.Genre = updatedBook.Genre;
            existingBook.IsAvailable = updatedBook.IsAvailable;

            await _context.SaveChangesAsync();

            return Ok(existingBook);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return BadRequest("Can't find requested index.");

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            var updatedList = await _context.Books.ToListAsync();
            return Ok(book);
        }


        [HttpGet("search")]
        public async Task<IActionResult> GetBooks(
    [FromQuery] string? title,
    [FromQuery] string? genre,
    [FromQuery] bool? sortByYearDescending)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(b => b.Title==title);
            }

            if (!string.IsNullOrWhiteSpace(genre))
            {
                query = query.Where(b => b.Genre.ToLower() == genre.ToLower());
            }

            if (sortByYearDescending.HasValue)
            {
                query = sortByYearDescending.Value
                    ? query.OrderByDescending(b => b.PublishYear)
                    : query.OrderBy(b => b.PublishYear); 
            }

            var result = await query.ToListAsync();
            return Ok(result);
        }
    }
}
