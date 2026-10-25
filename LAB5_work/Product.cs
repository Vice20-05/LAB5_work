using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace LAB5_work
{
    // Requirement 1: Entity Framework data model for a product in the shop
    public class Product
    {
        // unique identifier (EF makes "Id" the primary key, auto-increment)
        public int Id { get; set; }

        // name, max 30 characters
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        // description, max 500 characters
        [MaxLength(500)]
        public string Description { get; set; }

        // date the product entered the shop
        public DateTime EntryDate { get; set; }

        // expiration date
        public DateTime ExpirationDate { get; set; }

        // available quantity
        public int Quantity { get; set; }
    }

    // the database context, EF creates a "Products" table from the DbSet
    public class ProductDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
    }
}
