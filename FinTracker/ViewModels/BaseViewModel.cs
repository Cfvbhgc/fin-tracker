using CommunityToolkit.Mvvm.ComponentModel;

namespace FinTracker.ViewModels
{
    /// <summary>
    /// Base class for all view models in the FinTracker application.
    /// Inherits from <see cref="ObservableObject"/> provided by CommunityToolkit.Mvvm,
    /// which implements <see cref="System.ComponentModel.INotifyPropertyChanged"/> and
    /// <see cref="System.ComponentModel.INotifyPropertyChanging"/>.
    /// </summary>
    public abstract class BaseViewModel : ObservableObject
    {
        /// <summary>
        /// Gets or sets a value indicating whether the view model is currently loading data.
        /// </summary>
        private bool _isLoading;

        /// <summary>
        /// Gets or sets a value indicating whether the view model is currently loading data.
        /// When true, the UI may display a loading indicator.
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }
    }
}
