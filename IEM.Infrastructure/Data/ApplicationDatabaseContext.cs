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
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserConnection> UserConnections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.UserId);
                entity.HasOne(e => e.Role)
                .WithMany(e => e.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Users_Role");
            });

            modelBuilder.Entity<UserConnection>(entity =>
            {
                entity.HasKey(x => x.UserId);
                entity.HasOne(e => e.User)
                .WithMany(e => e.UserConnections)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserConnections_User");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);
            });

        }
    }
}
