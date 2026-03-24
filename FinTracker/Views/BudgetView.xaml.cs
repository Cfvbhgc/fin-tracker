using System.Windows.Controls;

namespace FinTracker.Views
{
    /// <summary>
    /// Interaction logic for BudgetView.xaml.
    /// Displays the budget management interface with progress bars showing spending
    /// against monthly budget limits for each category.
    /// </summary>
    public partial class BudgetView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BudgetView"/> class
        /// and loads the XAML-defined UI components.
        /// </summary>
        public BudgetView()
        {
            InitializeComponent();
        }
    }
}
