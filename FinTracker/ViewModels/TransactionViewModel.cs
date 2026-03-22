using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using FinTracker.Models;
using FinTracker.Services;

namespace FinTracker.ViewModels
{
    /// <summary>
    /// View model for the transaction management view.
    /// Provides properties and commands for displaying, adding, deleting, filtering,
    /// and exporting financial transactions.
    /// </summary>
    public class TransactionViewModel : BaseViewModel
    {
        /// <summary>
        /// The service responsible for transaction data access operations.
        /// </summary>
        private readonly TransactionService _transactionService;

        /// <summary>
        /// The service responsible for category data access operations.
        /// </summary>
        private readonly CategoryService _categoryService;

        /// <summary>
        /// The service responsible for exporting transactions to CSV format.
        /// </summary>
        private readonly CsvExportService _csvExportService;

        /// <summary>
        /// Backing field for the collection of transactions displayed in the view.
        /// </summary>
        private ObservableCollection<Transaction> _transactions;

        /// <summary>
        /// Backing field for the collection of available categories.
        /// </summary>
        private ObservableCollection<Category> _categories;

        /// <summary>
        /// Backing field for the amount entered in the new transaction form.
        /// </summary>
        private string _newTransactionAmount;

        /// <summary>
        /// Backing field for the description entered in the new transaction form.
        /// </summary>
        private string _newTransactionDescription;

        /// <summary>
        /// Backing field for the date selected in the new transaction form.
        /// </summary>
        private DateTime _newTransactionDate;

        /// <summary>
        /// Backing field for the category selected in the new transaction form.
        /// </summary>
        private Category? _selectedCategory;

        /// <summary>
        /// Backing field for the transaction type selected in the new transaction form.
        /// </summary>
        private TransactionType _selectedTransactionType;

        /// <summary>
        /// Backing field for the currently selected transaction in the data grid.
        /// </summary>
        private Transaction? _selectedTransaction;

        /// <summary>
        /// Backing field for the start date of the date range filter.
        /// </summary>
        private DateTime _filterStartDate;

        /// <summary>
        /// Backing field for the end date of the date range filter.
        /// </summary>
        private DateTime _filterEndDate;

        /// <summary>
        /// Backing field for the category selected in the filter controls.
        /// </summary>
        private Category? _filterCategory;

        /// <summary>
        /// Backing field for the computed total income amount.
        /// </summary>
        private decimal _totalIncome;

        /// <summary>
        /// Backing field for the computed total expense amount.
        /// </summary>
        private decimal _totalExpense;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionViewModel"/> class.
        /// Sets up services, initializes default values, and creates relay commands.
        /// </summary>
        public TransactionViewModel()
        {
            _transactionService = new TransactionService();
            _categoryService = new CategoryService();
            _csvExportService = new CsvExportService();

            _transactions = new ObservableCollection<Transaction>();
            _categories = new ObservableCollection<Category>();
            _newTransactionAmount = string.Empty;
            _newTransactionDescription = string.Empty;
            _newTransactionDate = DateTime.Today;
            _selectedTransactionType = TransactionType.Expense;
            _filterStartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            _filterEndDate = DateTime.Today;

            AddTransactionCommand = new AsyncRelayCommand(ExecuteAddTransactionAsync);
            DeleteTransactionCommand = new AsyncRelayCommand(ExecuteDeleteTransactionAsync);
            ApplyFilterCommand = new AsyncRelayCommand(ExecuteApplyFilterAsync);
            ExportToCsvCommand = new AsyncRelayCommand(ExecuteExportToCsvAsync);
            ClearFilterCommand = new AsyncRelayCommand(ExecuteClearFilterAsync);

            Task.Run(async () => await InitializeDataAsync());
        }

        /// <summary>
        /// Gets or sets the observable collection of transactions displayed in the data grid.
        /// </summary>
        public ObservableCollection<Transaction> Transactions
        {
            get { return _transactions; }
            set { SetProperty(ref _transactions, value); }
        }

        /// <summary>
        /// Gets or sets the observable collection of available categories for the category combo box.
        /// </summary>
        public ObservableCollection<Category> Categories
        {
            get { return _categories; }
            set { SetProperty(ref _categories, value); }
        }

        /// <summary>
        /// Gets or sets the amount text entered in the new transaction form.
        /// Stored as a string to allow validation before parsing to decimal.
        /// </summary>
        public string NewTransactionAmount
        {
            get { return _newTransactionAmount; }
            set { SetProperty(ref _newTransactionAmount, value); }
        }

        /// <summary>
        /// Gets or sets the description text entered in the new transaction form.
        /// </summary>
        public string NewTransactionDescription
        {
            get { return _newTransactionDescription; }
            set { SetProperty(ref _newTransactionDescription, value); }
        }

        /// <summary>
        /// Gets or sets the date selected in the new transaction form's date picker.
        /// </summary>
        public DateTime NewTransactionDate
        {
            get { return _newTransactionDate; }
            set { SetProperty(ref _newTransactionDate, value); }
        }

        /// <summary>
        /// Gets or sets the category selected in the new transaction form's combo box.
        /// </summary>
        public Category? SelectedCategory
        {
            get { return _selectedCategory; }
            set { SetProperty(ref _selectedCategory, value); }
        }

        /// <summary>
        /// Gets or sets the transaction type (Income or Expense) selected in the form.
        /// </summary>
        public TransactionType SelectedTransactionType
        {
            get { return _selectedTransactionType; }
            set { SetProperty(ref _selectedTransactionType, value); }
        }

        /// <summary>
        /// Gets or sets the currently selected transaction in the data grid.
        /// Used by the delete command to determine which transaction to remove.
        /// </summary>
        public Transaction? SelectedTransaction
        {
            get { return _selectedTransaction; }
            set { SetProperty(ref _selectedTransaction, value); }
        }

        /// <summary>
        /// Gets or sets the start date for the transaction filter range.
        /// </summary>
        public DateTime FilterStartDate
        {
            get { return _filterStartDate; }
            set { SetProperty(ref _filterStartDate, value); }
        }

        /// <summary>
        /// Gets or sets the end date for the transaction filter range.
        /// </summary>
        public DateTime FilterEndDate
        {
            get { return _filterEndDate; }
            set { SetProperty(ref _filterEndDate, value); }
        }

        /// <summary>
        /// Gets or sets the category selected for filtering transactions.
        /// If null, transactions from all categories are displayed.
        /// </summary>
        public Category? FilterCategory
        {
            get { return _filterCategory; }
            set { SetProperty(ref _filterCategory, value); }
        }

        /// <summary>
        /// Gets the computed total income from all currently displayed transactions.
        /// </summary>
        public decimal TotalIncome
        {
            get { return _totalIncome; }
            private set { SetProperty(ref _totalIncome, value); }
        }

        /// <summary>
        /// Gets the computed total expense from all currently displayed transactions.
        /// </summary>
        public decimal TotalExpense
        {
            get { return _totalExpense; }
            private set { SetProperty(ref _totalExpense, value); }
        }

        /// <summary>
        /// Gets the collection of available transaction types for the type selector.
        /// </summary>
        public IEnumerable<TransactionType> TransactionTypes
        {
            get { return Enum.GetValues(typeof(TransactionType)).Cast<TransactionType>(); }
        }

        /// <summary>
        /// Gets the async relay command that adds a new transaction to the database.
        /// </summary>
        public IAsyncRelayCommand AddTransactionCommand { get; }

        /// <summary>
        /// Gets the async relay command that deletes the currently selected transaction.
        /// </summary>
        public IAsyncRelayCommand DeleteTransactionCommand { get; }

        /// <summary>
        /// Gets the async relay command that applies the date and category filters.
        /// </summary>
        public IAsyncRelayCommand ApplyFilterCommand { get; }

        /// <summary>
        /// Gets the async relay command that exports the current transactions to a CSV file.
        /// </summary>
        public IAsyncRelayCommand ExportToCsvCommand { get; }

        /// <summary>
        /// Gets the async relay command that clears all filters and reloads all transactions.
        /// </summary>
        public IAsyncRelayCommand ClearFilterCommand { get; }

        /// <summary>
        /// Loads initial data (categories and transactions) from the database.
        /// Called once during view model construction.
        /// </summary>
        private async Task InitializeDataAsync()
        {
            List<Category> loadedCategories = await _categoryService.GetAllCategoriesAsync();
            List<Transaction> loadedTransactions = await _transactionService.GetAllTransactionsAsync();

            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                Categories = new ObservableCollection<Category>(loadedCategories);
                Transactions = new ObservableCollection<Transaction>(loadedTransactions);
                RecalculateTotals();
            });
        }

        /// <summary>
        /// Adds a new transaction based on the values entered in the form, then refreshes the list.
        /// Validates that the amount is a valid positive decimal and that a category is selected.
        /// </summary>
        private async Task ExecuteAddTransactionAsync()
        {
            if (!decimal.TryParse(NewTransactionAmount, out decimal parsedAmount) || parsedAmount <= 0)
            {
                return;
            }

            if (SelectedCategory == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(NewTransactionDescription))
            {
                return;
            }

            Transaction newTransaction = new Transaction
            {
                Amount = parsedAmount,
                Description = NewTransactionDescription.Trim(),
                Date = NewTransactionDate,
                CategoryId = SelectedCategory.Id,
                Type = SelectedTransactionType
            };

            await _transactionService.AddTransactionAsync(newTransaction);

            NewTransactionAmount = string.Empty;
            NewTransactionDescription = string.Empty;
            NewTransactionDate = DateTime.Today;
            SelectedTransactionType = TransactionType.Expense;

            await RefreshTransactionsAsync();
        }

        /// <summary>
        /// Deletes the currently selected transaction from the database, then refreshes the list.
        /// </summary>
        private async Task ExecuteDeleteTransactionAsync()
        {
            if (SelectedTransaction == null)
            {
                return;
            }

            await _transactionService.DeleteTransactionAsync(SelectedTransaction.Id);
            await RefreshTransactionsAsync();
        }

        /// <summary>
        /// Applies the current date range and category filters to retrieve matching transactions.
        /// </summary>
        private async Task ExecuteApplyFilterAsync()
        {
            int? filterCategoryId = FilterCategory?.Id;
            List<Transaction> filteredTransactions = await _transactionService
                .GetFilteredTransactionsAsync(FilterStartDate, FilterEndDate, filterCategoryId);

            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                Transactions = new ObservableCollection<Transaction>(filteredTransactions);
                RecalculateTotals();
            });
        }

        /// <summary>
        /// Clears all active filters and reloads the complete list of transactions.
        /// </summary>
        private async Task ExecuteClearFilterAsync()
        {
            FilterStartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            FilterEndDate = DateTime.Today;
            FilterCategory = null;
            await RefreshTransactionsAsync();
        }

        /// <summary>
        /// Opens a save file dialog and exports the currently displayed transactions to a CSV file.
        /// </summary>
        private async Task ExecuteExportToCsvAsync()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                DefaultExt = ".csv",
                FileName = $"FinTracker_Export_{DateTime.Today:yyyy-MM-dd}.csv"
            };

            bool? dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult == true)
            {
                await _csvExportService.ExportTransactionsToCsvAsync(Transactions, saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// Reloads all transactions from the database and updates the observable collection.
        /// </summary>
        private async Task RefreshTransactionsAsync()
        {
            List<Transaction> allTransactions = await _transactionService.GetAllTransactionsAsync();

            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                Transactions = new ObservableCollection<Transaction>(allTransactions);
                RecalculateTotals();
            });
        }

        /// <summary>
        /// Recalculates the <see cref="TotalIncome"/> and <see cref="TotalExpense"/> properties
        /// based on the currently displayed transactions.
        /// </summary>
        private void RecalculateTotals()
        {
            TotalIncome = Transactions
                .Where(transaction => transaction.Type == TransactionType.Income)
                .Sum(transaction => transaction.Amount);

            TotalExpense = Transactions
                .Where(transaction => transaction.Type == TransactionType.Expense)
                .Sum(transaction => transaction.Amount);
        }
    }
}
