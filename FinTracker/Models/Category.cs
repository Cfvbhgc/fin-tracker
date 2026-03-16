using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinTracker.Models
{
    /// <summary>
    /// Represents a category used to classify financial transactions.
    /// Categories help organize transactions into meaningful groups such as "Food", "Transport", or "Salary".
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Gets or sets the unique identifier for this category.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the category.
        /// For example, "Food", "Transport", "Entertainment".
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the icon identifier or emoji string associated with this category.
        /// Used for visual representation in the user interface.
        /// </summary>
        [MaxLength(50)]
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the collection of transactions that belong to this category.
        /// This is the inverse navigation property for the Transaction-Category relationship.
        /// </summary>
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
