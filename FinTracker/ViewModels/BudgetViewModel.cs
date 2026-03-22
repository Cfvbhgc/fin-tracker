using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using FinTracker.Data;
using FinTracker.Models;
using FinTracker.Services;

namespace FinTracker.ViewModels
{
    /// <summary>
    /// Represents a budget item enriched with computed usage information for display in the UI.
    /// Combines the raw budget data with the actual spending amount and percentage used.
    /// </summary>
    public class BudgetDisplayItem : BaseViewModel
    {
        /// <summary>
        /// Backing field for the underlying budget entity.
        /// </summary>
        private Budget _budget;

        /// <summary>
        /// Backing field for the amount spent in this budget's category during the budget month.
        /// </summary>
        private decimal _amountSpent;

        /// <summary>
        /// Backing field for the percentage of the budget that has been used.
        /// </summary>
        private double _usagePercentage;

        /// <summary>
        /// Gets or sets the underlying budget entity containing the category, limit, and month.
        /// </summary>
        public Budget Budget
        {
            get { return _budget; }
            set { SetProperty(ref _budget, value); }
        }

        /// <summary>
        /// Gets or sets the total amount spent in this budget's category during the budget month.
        /// </summary>
        public decimal AmountSpent
        {
            get { return _amountSpent; }
            set { SetProperty(ref _amountSpent, value); }
        }

        /// <summary>
        /// Gets or sets the percentage of the monthly budget limit that has been used.
        /// Ranges from 0.0 to 100.0 (or higher if overspent).
        /// </summary>
        public double UsagePercentage
        {
            get { return _usagePercentage; }
            set { SetProperty(ref _usagePercentage, value); }
        }

        /// <summary>
        /// Gets the display name combining the category icon and name.
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (Budget?.Category != null)
                {
                    return $"{Budget.Category.Icon} {Budget.Category.Name}";
                }
                return "Unknown Category";
            }
        }

        /// <summary>
        /// Gets the remaining amount text for display.
        /// </summary>
        public string RemainingText
        {
            get
            {
                decimal remainingAmount = Budget.MonthlyLimit - AmountSpent;
                if (remainingAmount >= 0)
                {
                    return $"${remainingAmount:F2} remaining";
                }
                return $"${Math.Abs(remainingAmount):F2} over budget";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BudgetDisplayItem"/> class.
        /// </summary>
        public BudgetDisplayItem()
        {
            _budget = new Budget();
        }
    }

    /// <summary>
    /// View model for the budget management view.
    /// Provides properties and commands for displaying budgets with usage progress,
    /// adding new budgets, and editing existing budget limits.
    /// </summary>
    public class BudgetViewModel : BaseViewModel
    {
        /// <summary>
        /// The service responsible for transaction data access (used to calculate spending).
        /// </summary>
        private readonly TransactionService _transactionService;

        /// <summary>
        /// The service responsible for category data access.
        /// </summary>
        private readonly CategoryService _categoryService;

        /// <summary>
        /// Backing field for the collection of budget display items shown in the list view.
        /// </summary>
        private ObservableCollection<BudgetDisplayItem> _budgetDisplayItems;

        /// <summary>
        /// Backing field for the collection of available categories.
        /// </summary>
        private ObservableCollection<Category> _categories;

        /// <summary>
        /// Backing field for the category selected when adding a new budget.
        /// </summary>
        private Category? _newBudgetCategory;

        /// <summary>
        /// Backing field for the monthly limit entered when adding a new budget.
        /// </summary>
        private string _newBudgetMonthlyLimit;

        /// <summary>
        /// Backing field for the month selected when adding a new budget.
        /// </summary>
        private DateTime _selectedBudgetMonth;

        /// <summary>
        /// Backing field for the currently selected budget display item.
        /// </summary>
        private BudgetDisplayItem? _selectedBudgetItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="BudgetViewModel"/> class.
        /// Sets up services, initializes collections, and loads initial data.
        /// </summary>
        public BudgetViewModel()
        {
            _transactionService = new TransactionService();
            _categoryService = new CategoryService();

            _budgetDisplayItems = new ObservableCollection<BudgetDisplayItem>();
            _categories = new ObservableCollection<Category>();
            _newBudgetMonthlyLimit = string.Empty;
            _selectedBudgetMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            AddBudgetCommand = new AsyncRelayCommand(ExecuteAddBudgetAsync);
            DeleteBudgetCommand = new AsyncRelayCommand(ExecuteDeleteBudgetAsync);
            RefreshBudgetsCommand = new AsyncRelayCommand(ExecuteRefreshBudgetsAsync);

            Task.Run(async () => await InitializeBudgetDataAsync());
        }

        /// <summary>
        /// Gets or sets the observable collection of budget display items with usage information.
        /// </summary>
        public ObservableCollection<BudgetDisplayItem> BudgetDisplayItems
        {
            get { return _budgetDisplayItems; }
            set { SetProperty(ref _budgetDisplayItems, value); }
        }

        /// <summary>
        /// Gets or sets the observable collection of available categories for the budget form.
        /// </summary>
        public ObservableCollection<Category> Categories
        {
            get { return _categories; }
            set { SetProperty(ref _categories, value); }
        }

        /// <summary>
        /// Gets or sets the category selected in the new budget form.
        /// </summary>
        public Category? NewBudgetCategory
        {
            get { return _newBudgetCategory; }
            set { SetProperty(ref _newBudgetCategory, value); }
        }

        /// <summary>
        /// Gets or sets the monthly limit text entered in the new budget form.
        /// Stored as a string to allow validation before parsing.
        /// </summary>
        public string NewBudgetMonthlyLimit
        {
            get { return _newBudgetMonthlyLimit; }
            set { SetProperty(ref _newBudgetMonthlyLimit, value); }
        }

        /// <summary>
        /// Gets or sets the month and year for the new budget.
        /// </summary>
        public DateTime SelectedBudgetMonth
        {
            get { return _selectedBudgetMonth; }
            set { SetProperty(ref _selectedBudgetMonth, value); }
        }

        /// <summary>
        /// Gets or sets the currently selected budget display item in the list view.
        /// </summary>
        public BudgetDisplayItem? SelectedBudgetItem
        {
            get { return _selectedBudgetItem; }
            set { SetProperty(ref _selectedBudgetItem, value); }
        }

        /// <summary>
        /// Gets the async relay command that adds a new budget to the database.
        /// </summary>
        public IAsyncRelayCommand AddBudgetCommand { get; }

        /// <summary>
        /// Gets the async relay command that deletes the selected budget.
        /// </summary>
        public IAsyncRelayCommand DeleteBudgetCommand { get; }

        /// <summary>
        /// Gets the async relay command that refreshes the budget list with current spending data.
        /// </summary>
        public IAsyncRelayCommand RefreshBudgetsCommand { get; }

        /// <summary>
        /// Loads categories and budgets from the database during initialization.
        /// </summary>
        private async Task InitializeBudgetDataAsync()
        {
            List<Category> loadedCategories = await _categoryService.GetAllCategoriesAsync();

            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                Categories = new ObservableCollection<Category>(loadedCategories);
            });

            await ExecuteRefreshBudgetsAsync();
        }

        /// <summary>
        /// Adds a new budget entry for the selected category and month, then refreshes the list.
        /// Validates that a category is selected and the monthly limit is a valid positive decimal.
        /// </summary>
        private async Task ExecuteAddBudgetAsync()
        {
            if (NewBudgetCategory == null)
            {
                return;
            }

            if (!decimal.TryParse(NewBudgetMonthlyLimit, out decimal parsedLimit) || parsedLimit <= 0)
            {
                return;
            }

            Budget newBudget = new Budget
            {
                CategoryId = NewBudgetCategory.Id,
                MonthlyLimit = parsedLimit,
                Month = new DateTime(SelectedBudgetMonth.Year, SelectedBudgetMonth.Month, 1)
            };

            using FinTrackerDbContext databaseContext = new FinTrackerDbContext();
            databaseContext.Budgets.Add(newBudget);
            await databaseContext.SaveChangesAsync();

            NewBudgetMonthlyLimit = string.Empty;

            await ExecuteRefreshBudgetsAsync();
        }

        /// <summary>
        /// Deletes the currently selected budget from the database, then refreshes the list.
        /// </summary>
        private async Task ExecuteDeleteBudgetAsync()
        {
            if (SelectedBudgetItem == null)
            {
                return;
            }

            using FinTrackerDbContext databaseContext = new FinTrackerDbContext();
            Budget? budgetToDelete = await databaseContext.Budgets
                .FirstOrDefaultAsync(budget => budget.Id == SelectedBudgetItem.Budget.Id);

            if (budgetToDelete != null)
            {
                databaseContext.Budgets.Remove(budgetToDelete);
                await databaseContext.SaveChangesAsync();
            }

            await ExecuteRefreshBudgetsAsync();
        }

        /// <summary>
        /// Reloads all budgets from the database and calculates the current spending
        /// for each budget's category and month to determine usage percentages.
        /// </summary>
        private async Task ExecuteRefreshBudgetsAsync()
        {
            using FinTrackerDbContext databaseContext = new FinTrackerDbContext();
            List<Budget> allBudgets = await databaseContext.Budgets
                .Include(budget => budget.Category)
                .OrderByDescending(budget => budget.Month)
                .ThenBy(budget => budget.Category!.Name)
                .ToListAsync();

            List<BudgetDisplayItem> displayItems = new List<BudgetDisplayItem>();

            foreach (Budget currentBudget in allBudgets)
            {
                decimal categorySpending = await _transactionService
                    .GetCategorySpendingForMonthAsync(currentBudget.CategoryId, currentBudget.Month);

                double calculatedUsagePercentage = currentBudget.MonthlyLimit > 0
                    ? (double)(categorySpending / currentBudget.MonthlyLimit) * 100.0
                    : 0.0;

                BudgetDisplayItem displayItem = new BudgetDisplayItem
                {
                    Budget = currentBudget,
                    AmountSpent = categorySpending,
                    UsagePercentage = Math.Min(calculatedUsagePercentage, 100.0)
                };

                displayItems.Add(displayItem);
            }

            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                BudgetDisplayItems = new ObservableCollection<BudgetDisplayItem>(displayItems);
            });
        }
    }
}
