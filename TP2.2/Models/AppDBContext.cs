using Microsoft.EntityFrameworkCore;

namespace TP2._2.Models
{
    public class AppDBContext: DbContext

    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
    }
}
