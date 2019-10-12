using DotNetCore30Demo.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DotNetCore30Demo.Repository
{
    public class ChimpDbContext : BaseDbContext
    {
        public ChimpDbContext(DbContextOptions options):base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
