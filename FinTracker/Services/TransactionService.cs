using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FinTracker.Data;
using FinTracker.Models;

namespace FinTracker.Services
{
    /// <summary>
    /// Provides data access operations for <see cref="Transaction"/> entities.
    /// Encapsulates all CRUD (Create, Read, Update, Delete) logic for transactions
    /// using Entity Framework Core and the application's SQLite database.
    /// </summary>
    public class TransactionService
    {
        /// <summary>
        /// Retrieves all transactions from the database, including their associated category information.
        /// Results are ordered by date in descending order (most recent first).
        /// </summary>
        /// <returns>A list of all transactions with their categories loaded.</returns>
        public async Task<List<Transaction>> GetAllTransactionsAsync()
        {
            using FinTrackerDbContext databaseContext = new FinTrackerDbContext();
            List<Transaction> allTransactions = await databaseContext.Transactions
                .Include(transaction => transaction.Category)
                .OrderByDescending(transaction => transaction.Date)
                .ToListAsync();
            return allTransactions;
        }

        /// <summary>
        /// Retrieves transactions filtered by the specified date range and optional category.
        /// </summary>
        /// <param name="startDate">The inclusive start date of the filter range.</param>
        /// <param name="endDate">The inclusive end date of the filter range.</param>
        /// <param name="categoryId">The optional category identifier to filter by. If null, all categories are included.</param>
        /// <returns>A filtered list of transactions matching the specified criteria.</returns>
        public async Task<List<Transaction>> GetFilteredTransactionsAsync(DateTime startDate, DateTime endDate, int? categoryId)
        {
            using FinTrackerDbContext databaseContext = new FinTrackerDbContext();
            IQueryable<Transaction> transactionQuery = databaseContext.Transactions
                .Include(transaction => transaction.Category)
                .Where(transaction => transaction.Date >= startDate && transaction.Date <= endDate);

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                transactionQuery = transactionQuery.Where(transaction => transaction.CategoryId == categoryId.Value);
            }

            List<Transaction> filteredTransactions = await transactionQuery
                .OrderByDescending(transaction => transaction.Date)
                .ToListAsync();
            return filteredTransactions;
        }

        /// <summary>
        /// Adds a new transaction to the database.
        /// </summary>
        /// <param name="newTransaction">The transaction entity to add.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public async Task AddTransactionAsync(Transaction newTransaction)
        {
            using FinTrackerDbContext databaseContext = new FinTrackerDbContext();
            databaseContext.Transactions.Add(newTransaction);
            await databaseContext.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing transaction in the database.
        /// </summary>
        /// <param name="updatedTransaction">The transaction entity with updated values.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public async Task UpdateTransactionAsync(Transaction updatedTransaction)
        {
            using FinTrackerDbContext databaseContext = new FinTrackerDbContext();
            databaseContext.Transactions.Update(updatedTransaction);
            await databaseContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a transaction from the database by its unique identifier.
        /// </summary>
        /// <param name="transactionId">The unique identifier of the transaction to delete.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        public async Task DeleteTransactionAsync(int transactionId)
        {
            using FinTrackerDbContext databaseContext = new FinTrackerDbContext();
            Transaction? transactionToDelete = await databaseContext.Transactions
                .FirstOrDefaultAsync(transaction => transaction.Id == transactionId);

            if (transactionToDelete != null)
            {
                databaseContext.Transactions.Remove(transactionToDelete);
                await databaseContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Calculates the total amount spent in a specific category during a given month.
        /// Only expense-type transactions are included in the calculation.
        /// </summary>
        /// <param name="categoryId">The category identifier to calculate spending for.</param>
        /// <param name="month">The month to calculate spending for (only year and month are used).</param>
        /// <returns>The total expense amount for the specified category and month.</returns>
        public async Task<decimal> GetCategorySpendingForMonthAsync(int categoryId, DateTime month)
        {
            using FinTrackerDbContext databaseContext = new FinTrackerDbContext();
            decimal totalSpending = await databaseContext.Transactions
                .Where(transaction => transaction.CategoryId == categoryId
                    && transaction.Type == TransactionType.Expense
                    && transaction.Date.Year == month.Year
                    && transaction.Date.Month == month.Month)
                .SumAsync(transaction => transaction.Amount);
            return totalSpending;
        }
    }
}
