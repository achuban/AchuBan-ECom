using AchuBan_ECom.Models;
using AchuBan_ECom.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AchuBan_ECom.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
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



            modelBuilder.Entity<Product>()
                .Property(p => p.ListPrice)
                .HasPrecision(18, 2); // Adjust as needed

            modelBuilder.Entity<Product>()
                .Property(p => p.Price100)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price50)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Product 1", Description = "Description 1", ISBN = "ISBN-123", Author = "Mr. Max seed", ListPrice = 10.00m, Price50 = 9.00m, Price100 = 8.00m, CategoryId = 1, category = null! ,ImageUrl=null},
                new Product { Id = 2, Name = "Product 2", Description = "Description 2", ISBN = "ISBN-456", Author = "Mr. Seed 123", ListPrice = 20.00m, Price50 = 18.00m, Price100 = 16.00m, CategoryId = 2, category = null!, ImageUrl = null },
                new Product { Id = 3, Name = "Product 3", Description = "Description 3", ISBN = "ISBN-789", Author = "Mr. Mooc seed", ListPrice = 30.00m, Price50 = 27.00m, Price100 = 24.00m, CategoryId = 3, category = null!, ImageUrl = null },
                new Product { Id = 4, Name = "Product 4", Description = "Description 4", ISBN = "ISBN-101", Author = "Mr. Mussie seed", ListPrice = 40.00m, Price50 = 36.00m, Price100 = 32.00m, CategoryId = 1, category = null!, ImageUrl = null },
                new Product { Id = 5, Name = "Product 5", Description = "Description 5", ISBN = "ISBN-102", Author = "Mr. Max seed", ListPrice = 50.00m, Price50 = 45.00m, Price100 = 40.00m, CategoryId = 2, category = null!, ImageUrl = null },
                new Product { Id = 6, Name = "Product 6", Description = "Description 6", ISBN = "ISBN-103", Author = "Mr. Haile seed", ListPrice = 60.00m, Price50 = 54.00m, Price100 = 48.00m, CategoryId = 3, category = null!, ImageUrl = null },
                new Product { Id = 7, Name = "Product 7", Description = "Description 7", ISBN = "ISBN-104", Author = "Mr. Max seed", ListPrice = 70.00m, Price50 = 63.00m, Price100 = 56.00m, CategoryId = 1, category = null!, ImageUrl = null },
                new Product { Id = 8, Name = "Product 8", Description = "Description 8", ISBN = "ISBN-105", Author = "Mr. Assin seed", ListPrice = 80.00m, Price50 = 72.00m, Price100 = 64.00m, CategoryId = 2, category = null!, ImageUrl = null }
            );
        }
    }

}
