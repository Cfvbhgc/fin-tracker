using System.Windows.Controls;

namespace FinTracker.Views
{
    /// <summary>
    /// Interaction logic for TransactionView.xaml.
    /// Displays the transaction management interface including the data grid,
    /// add transaction form, and filter controls.
    /// </summary>
    public partial class TransactionView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionView"/> class
        /// and loads the XAML-defined UI components.
        /// </summary>
        public TransactionView()
        {
            InitializeComponent();
        }
    }
}
