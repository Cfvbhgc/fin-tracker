using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTracker.Models
{
    /// <summary>
    /// Represents the type of a financial transaction.
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Represents money received (e.g., salary, freelance payment).
        /// </summary>
        Income = 0,

        /// <summary>
        /// Represents money spent (e.g., groceries, rent).
        /// </summary>
        Expense = 1
    }

    /// <summary>
    /// Represents a single financial transaction recorded by the user.
    /// Each transaction has an amount, description, date, category, and type (income or expense).
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Gets or sets the unique identifier for this transaction.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the monetary amount of the transaction.
        /// Positive values represent the magnitude of income or expense.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets a human-readable description of the transaction.
        /// For example, "Weekly grocery shopping" or "Monthly salary deposit".
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the transaction occurred.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the foreign key identifier for the associated category.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the category this transaction belongs to.
        /// </summary>
        public Category? Category { get; set; }

        /// <summary>
        /// Gets or sets the type of this transaction, indicating whether it is income or an expense.
        /// </summary>
        public TransactionType Type { get; set; }
    }
}
