using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using FinTracker.Views;

namespace FinTracker.ViewModels
{
    /// <summary>
    /// The primary view model for the application's main window.
    /// Manages navigation between different views (Transactions, Budgets, Charts)
    /// by swapping the <see cref="CurrentView"/> property bound to the main content area.
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        /// <summary>
        /// Backing field for the currently displayed user control in the main content area.
        /// </summary>
        private UserControl _currentView;

        /// <summary>
        /// Backing field for the title text displayed in the navigation header.
        /// </summary>
        private string _currentViewTitle;

        /// <summary>
        /// The cached instance of the transactions view to avoid repeated instantiation.
        /// </summary>
        private readonly TransactionView _transactionView;

        /// <summary>
        /// The cached instance of the budget view to avoid repeated instantiation.
        /// </summary>
        private readonly BudgetView _budgetView;

        /// <summary>
        /// The cached instance of the chart view to avoid repeated instantiation.
        /// </summary>
        private readonly ChartView _chartView;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// Creates cached instances of each view and sets the default view to Transactions.
        /// </summary>
        public MainViewModel()
        {
            _transactionView = new TransactionView();
            _budgetView = new BudgetView();
            _chartView = new ChartView();

            _currentView = _transactionView;
            _currentViewTitle = "Transactions";

            NavigateToTransactionsCommand = new RelayCommand(ExecuteNavigateToTransactions);
            NavigateToBudgetsCommand = new RelayCommand(ExecuteNavigateToBudgets);
            NavigateToChartsCommand = new RelayCommand(ExecuteNavigateToCharts);
        }

        /// <summary>
        /// Gets or sets the user control currently displayed in the main window's content area.
        /// Changing this property triggers a view switch in the UI.
        /// </summary>
        public UserControl CurrentView
        {
            get { return _currentView; }
            set { SetProperty(ref _currentView, value); }
        }

        /// <summary>
        /// Gets or sets the title text displayed above the current view content.
        /// </summary>
        public string CurrentViewTitle
        {
            get { return _currentViewTitle; }
            set { SetProperty(ref _currentViewTitle, value); }
        }

        /// <summary>
        /// Gets the relay command that navigates to the Transactions view.
        /// </summary>
        public IRelayCommand NavigateToTransactionsCommand { get; }

        /// <summary>
        /// Gets the relay command that navigates to the Budgets view.
        /// </summary>
        public IRelayCommand NavigateToBudgetsCommand { get; }

        /// <summary>
        /// Gets the relay command that navigates to the Charts view.
        /// </summary>
        public IRelayCommand NavigateToChartsCommand { get; }

        /// <summary>
        /// Switches the main content area to display the Transactions view.
        /// </summary>
        private void ExecuteNavigateToTransactions()
        {
            CurrentView = _transactionView;
            CurrentViewTitle = "Transactions";
        }

        /// <summary>
        /// Switches the main content area to display the Budgets view.
        /// </summary>
        private void ExecuteNavigateToBudgets()
        {
            CurrentView = _budgetView;
            CurrentViewTitle = "Budgets";
        }

        /// <summary>
        /// Switches the main content area to display the Charts view.
        /// </summary>
        private void ExecuteNavigateToCharts()
        {
            CurrentView = _chartView;
            CurrentViewTitle = "Charts";
        }
    }
}
