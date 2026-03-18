using System;
using Microsoft.EntityFrameworkCore;
using FinTracker.Models;

namespace FinTracker.Data
{
    /// <summary>
    /// Entity Framework Core database context for the FinTracker application.
    /// Manages the connection to the SQLite database and provides DbSet properties
    /// for querying and saving instances of the application's data models.
    /// </summary>
    public class FinTrackerDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the DbSet for accessing and managing <see cref="Transaction"/> entities.
        /// </summary>
        public DbSet<Transaction> Transactions { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for accessing and managing <see cref="Category"/> entities.
        /// </summary>
        public DbSet<Category> Categories { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for accessing and managing <see cref="Budget"/> entities.
        /// </summary>
        public DbSet<Budget> Budgets { get; set; } = null!;

        /// <summary>
        /// Configures the database connection to use SQLite with a local file named "fintracker.db".
        /// </summary>
        /// <param name="optionsBuilder">The options builder used to configure the context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=fintracker.db");
            }
        }

        /// <summary>
        /// Configures the entity model relationships and seeds the database with default categories.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure entity mappings.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasOne(transaction => transaction.Category)
                      .WithMany(category => category.Transactions)
                      .HasForeignKey(transaction => transaction.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Budget>(entity =>
            {
                entity.HasOne(budget => budget.Category)
                      .WithMany()
                      .HasForeignKey(budget => budget.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Food", Icon = "\ud83c\udf54" },
                new Category { Id = 2, Name = "Transport", Icon = "\ud83d\ude8c" },
                new Category { Id = 3, Name = "Entertainment", Icon = "\ud83c\udfac" },
                new Category { Id = 4, Name = "Salary", Icon = "\ud83d\udcb0" },
                new Category { Id = 5, Name = "Shopping", Icon = "\ud83d\uded2" },
                new Category { Id = 6, Name = "Healthcare", Icon = "\ud83c\udfe5" },
                new Category { Id = 7, Name = "Utilities", Icon = "\ud83d\udca1" },
                new Category { Id = 8, Name = "Freelance", Icon = "\ud83d\udcbb" },
                new Category { Id = 9, Name = "Investments", Icon = "\ud83d\udcc8" },
                new Category { Id = 10, Name = "Other", Icon = "\ud83d\udccb" }
            );
        }
    }
}
