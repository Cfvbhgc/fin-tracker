using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using FinTracker.Data;
using FinTracker.Models;

namespace FinTracker.Views
{
    /// <summary>
    /// Interaction logic for ChartView.xaml.
    /// Displays visual charts including a pie chart of expenses by category
    /// and a bar chart comparing monthly income versus expenses.
    /// Chart data is loaded directly from the database on initialization.
    /// </summary>
    public partial class ChartView : UserControl
    {
        /// <summary>
        /// A predefined palette of colors used for the pie chart slices.
        /// Each category is assigned a distinct color for visual clarity.
        /// </summary>
        private static readonly SKColor[] ChartColorPalette = new SKColor[]
        {
            SKColor.Parse("#2563EB"),
            SKColor.Parse("#10B981"),
            SKColor.Parse("#F59E0B"),
            SKColor.Parse("#EF4444"),
            SKColor.Parse("#8B5CF6"),
            SKColor.Parse("#EC4899"),
            SKColor.Parse("#06B6D4"),
            SKColor.Parse("#84CC16"),
            SKColor.Parse("#F97316"),
            SKColor.Parse("#6366F1")
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartView"/> class,
        /// loads the XAML components, and populates the charts with data from the database.
        /// </summary>
        public ChartView()
        {
            InitializeComponent();
            LoadChartData();
        }

        /// <summary>
        /// Loads transaction data from the database and populates both the pie chart
        /// (expenses by category) and the bar chart (monthly income vs expenses).
        /// </summary>
        private void LoadChartData()
        {
            using FinTrackerDbContext databaseContext = new FinTrackerDbContext();

            List<Transaction> allTransactions = databaseContext.Transactions
                .Include(transaction => transaction.Category)
                .ToList();

            BuildExpensePieChart(allTransactions);
            BuildMonthlyBarChart(allTransactions);
        }

        /// <summary>
        /// Creates and assigns the pie chart series showing expense totals grouped by category.
        /// Only expense-type transactions are included in the pie chart.
        /// </summary>
        /// <param name="allTransactions">The complete list of transactions from the database.</param>
        private void BuildExpensePieChart(List<Transaction> allTransactions)
        {
            var expensesByCategory = allTransactions
                .Where(transaction => transaction.Type == TransactionType.Expense)
                .GroupBy(transaction => transaction.Category?.Name ?? "Uncategorized")
                .Select(categoryGroup => new
                {
                    CategoryName = categoryGroup.Key,
                    TotalAmount = categoryGroup.Sum(transaction => (double)transaction.Amount)
                })
                .OrderByDescending(group => group.TotalAmount)
                .ToList();

            List<ISeries> pieSeriesCollection = new List<ISeries>();
            int colorIndex = 0;

            foreach (var categoryExpense in expensesByCategory)
            {
                SKColor sliceColor = ChartColorPalette[colorIndex % ChartColorPalette.Length];

                PieSeries<double> pieSeries = new PieSeries<double>
                {
                    Values = new double[] { categoryExpense.TotalAmount },
                    Name = categoryExpense.CategoryName,
                    Fill = new SolidColorPaint(sliceColor),
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsFormatter = chartPoint =>
                        $"{categoryExpense.CategoryName}: ${categoryExpense.TotalAmount:N2}"
                };

                pieSeriesCollection.Add(pieSeries);
                colorIndex++;
            }

            if (pieSeriesCollection.Count == 0)
            {
                PieSeries<double> emptyPlaceholderSeries = new PieSeries<double>
                {
                    Values = new double[] { 1 },
                    Name = "No expenses yet",
                    Fill = new SolidColorPaint(SKColor.Parse("#E2E8F0"))
                };
                pieSeriesCollection.Add(emptyPlaceholderSeries);
            }

            ExpensePieChart.Series = pieSeriesCollection;
        }

        /// <summary>
        /// Creates and assigns the bar chart series showing monthly income and expense totals.
        /// Groups transactions by month and displays income and expenses as side-by-side bars.
        /// </summary>
        /// <param name="allTransactions">The complete list of transactions from the database.</param>
        private void BuildMonthlyBarChart(List<Transaction> allTransactions)
        {
            var transactionsGroupedByMonth = allTransactions
                .GroupBy(transaction => new DateTime(transaction.Date.Year, transaction.Date.Month, 1))
                .OrderBy(monthGroup => monthGroup.Key)
                .ToList();

            if (transactionsGroupedByMonth.Count == 0)
            {
                DateTime currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                MonthlyBarChart.Series = new ISeries[]
                {
                    new ColumnSeries<double>
                    {
                        Values = new double[] { 0 },
                        Name = "Income",
                        Fill = new SolidColorPaint(SKColor.Parse("#10B981"))
                    },
                    new ColumnSeries<double>
                    {
                        Values = new double[] { 0 },
                        Name = "Expenses",
                        Fill = new SolidColorPaint(SKColor.Parse("#EF4444"))
                    }
                };

                MonthlyBarChart.XAxes = new Axis[]
                {
                    new Axis
                    {
                        Labels = new string[] { currentMonth.ToString("MMM yyyy") },
                        LabelsRotation = 0
                    }
                };
                return;
            }

            List<double> monthlyIncomeValues = new List<double>();
            List<double> monthlyExpenseValues = new List<double>();
            List<string> monthLabels = new List<string>();

            foreach (var monthGroup in transactionsGroupedByMonth)
            {
                double monthlyIncomeTotal = (double)monthGroup
                    .Where(transaction => transaction.Type == TransactionType.Income)
                    .Sum(transaction => transaction.Amount);

                double monthlyExpenseTotal = (double)monthGroup
                    .Where(transaction => transaction.Type == TransactionType.Expense)
                    .Sum(transaction => transaction.Amount);

                monthlyIncomeValues.Add(monthlyIncomeTotal);
                monthlyExpenseValues.Add(monthlyExpenseTotal);
                monthLabels.Add(monthGroup.Key.ToString("MMM yyyy"));
            }

            ColumnSeries<double> incomeBarSeries = new ColumnSeries<double>
            {
                Values = monthlyIncomeValues.ToArray(),
                Name = "Income",
                Fill = new SolidColorPaint(SKColor.Parse("#10B981")),
                MaxBarWidth = 30
            };

            ColumnSeries<double> expenseBarSeries = new ColumnSeries<double>
            {
                Values = monthlyExpenseValues.ToArray(),
                Name = "Expenses",
                Fill = new SolidColorPaint(SKColor.Parse("#EF4444")),
                MaxBarWidth = 30
            };

            MonthlyBarChart.Series = new ISeries[] { incomeBarSeries, expenseBarSeries };

            MonthlyBarChart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = monthLabels.ToArray(),
                    LabelsRotation = 0
                }
            };

            MonthlyBarChart.YAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => $"${value:N0}"
                }
            };
        }
    }
}
