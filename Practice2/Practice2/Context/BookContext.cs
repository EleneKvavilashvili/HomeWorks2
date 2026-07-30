using Microsoft.EntityFrameworkCore;
using Practice2.Models;

namespace Practice2.Context
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
    }
}
