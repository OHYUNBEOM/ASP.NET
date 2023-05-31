using Microsoft.EntityFrameworkCore;

namespace TodoAPIServer.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
