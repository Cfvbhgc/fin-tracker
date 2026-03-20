using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FinTracker.Data;
using FinTracker.Models;

namespace FinTracker.Services
{
    /// <summary>
    /// Provides data access operations for <see cref="Category"/> entities.
    /// Handles retrieval of transaction categories from the SQLite database.
    /// </summary>
    public class CategoryService
    {
        /// <summary>
        /// Retrieves all categories from the database, ordered alphabetically by name.
        /// </summary>
        /// <returns>A list of all available categories.</returns>
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            using FinTrackerDbContext databaseContext = new FinTrackerDbContext();
            List<Category> allCategories = await databaseContext.Categories
                .OrderBy(category => category.Name)
                .ToListAsync();
            return allCategories;
        }

        /// <summary>
        /// Retrieves a single category by its unique identifier.
        /// </summary>
        /// <param name="categoryId">The unique identifier of the category to retrieve.</param>
        /// <returns>The category if found; otherwise, null.</returns>
        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            using FinTrackerDbContext databaseContext = new FinTrackerDbContext();
            Category? foundCategory = await databaseContext.Categories
                .FirstOrDefaultAsync(category => category.Id == categoryId);
            return foundCategory;
        }
    }
}
