using System.Windows;
using Microsoft.EntityFrameworkCore;
using FinTracker.Data;

namespace FinTracker
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// Handles application startup including database initialization and migration.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Raises the <see cref="Application.Startup"/> event and ensures the SQLite database
        /// is created with all seed data before the main window is shown.
        /// </summary>
        /// <param name="e">The startup event arguments.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using FinTrackerDbContext databaseContext = new FinTrackerDbContext();
            databaseContext.Database.EnsureCreated();
        }
    }
}
