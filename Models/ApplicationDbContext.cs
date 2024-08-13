using Microsoft.EntityFrameworkCore;

namespace Budget_Management_System.Models
{
    public class ApplicationDbContext : DbContext
    { 
        public ApplicationDbContext(DbContextOptions contextOptions):base(contextOptions)
        {

        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
