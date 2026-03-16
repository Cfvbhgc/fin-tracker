using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTracker.Models
{
    /// <summary>
    /// Represents a monthly budget limit for a specific spending category.
    /// Budgets allow users to set spending caps and track how much of their budget has been used.
    /// </summary>
    public class Budget
    {
        /// <summary>
        /// Gets or sets the unique identifier for this budget entry.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the foreign key identifier for the category this budget applies to.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the category this budget is associated with.
        /// </summary>
        public Category? Category { get; set; }

        /// <summary>
        /// Gets or sets the maximum amount of money allocated for this category in the given month.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyLimit { get; set; }

        /// <summary>
        /// Gets or sets the month and year this budget applies to.
        /// Only the year and month components are significant; the day is typically set to 1.
        /// </summary>
        public DateTime Month { get; set; }
    }
}
