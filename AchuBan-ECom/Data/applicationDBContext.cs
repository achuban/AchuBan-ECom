using AchuBan_ECom.Models;
using Microsoft.EntityFrameworkCore;

namespace AchuBan_ECom.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>()
                .HasIndex(e => e.Name)
                .IsUnique();
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", Description = "Action Movies", displayOrder = 1 },
                new Category { Id = 2, Name = "Comedy", Description = "Comedy Movies", displayOrder = 2 },
                new Category { Id = 3, Name = "Drama", Description = "Drama Movies", displayOrder = 3 }
            );
        }
    }

}
