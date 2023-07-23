using WebShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebShop.DataAcess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products{ get; set; }
        public DbSet<PromoCode> PromoCodes{ get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ChatRoom> ChatRoom { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //needed because keys of identity table are mapped in the OnModelCreating of identity dbcontext
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Parfume", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Lora", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 }
                );

            modelBuilder.Entity<Company>().HasData(
                new Company { Id = 1, Name = "Solution", StreetAddress="123", City="City",
                                PostalCode="12121", State="NY", PhoneNumber="990000"},
                new Company {
                    Id = 2,
                    Name = "Vivid",
                    StreetAddress = "Vid St",
                    City = "City",
                    PostalCode = "66666",
                    State = "IL",
                    PhoneNumber = "770000"
                },
                new Company {
                    Id = 3,
                    Name = "Readers",
                    StreetAddress = "St",
                    City = "land",
                    PostalCode = "99999",
                    State = "NY",
                    PhoneNumber = "1113335555"
                }
                );


            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Time",
                    Author = "Billy",
                    Description = "Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "SWD991",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Title = "Skies",
                    Author = "Hoover",
                    Description = "Shit",
                    ISBN = "CAd77701",
                    ListPrice = 40,
                    Price = 30,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 3,
                    Title = "Sunset",
                    Author = "Button",
                    Description = "Praesent vitae sodales libero.",
                    ISBN = "RITO5501",
                    ListPrice = 55,
                    Price = 50,
                    Price50 = 40,
                    Price100 = 35,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 4,
                    Title = "Candy",
                    Author = "Muscles",
                    Description = "adfsadsadsadsad",
                    ISBN = "WS3333333301",
                    ListPrice = 70,
                    Price = 65,
                    Price50 = 60,
                    Price100 = 55,
                    CategoryId = 2
                },
                new Product
                {
                    Id = 5,
                    Title = "Ocean",
                    Author = "Ron",
                    Description = "ddasdasdasdd",
                    ISBN = "SOTJ1101",
                    ListPrice = 30,
                    Price = 27,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId = 2
                },
                new Product
                {
                    Id = 6,
                    Title = "Wonders",
                    Author = "Phantom",
                    Description = "131sssssssssss",
                    ISBN = "FOT00001",
                    ListPrice = 25,
                    Price = 23,
                    Price50 = 22,
                    Price100 = 20,
                    CategoryId = 3
                }
                );
        }
    }
}
