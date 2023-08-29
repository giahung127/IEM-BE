using IEM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IEM.Infrastructure.Data
{
    public class ApplicationDatabaseContext : DbContext
    {
        public ApplicationDatabaseContext(DbContextOptions<ApplicationDatabaseContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
