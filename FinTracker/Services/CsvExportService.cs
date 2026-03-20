using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FinTracker.Models;

namespace FinTracker.Services
{
    /// <summary>
    /// Provides functionality to export transaction data to CSV (Comma-Separated Values) files.
    /// The exported file includes headers and all relevant transaction fields formatted for
    /// easy import into spreadsheet applications.
    /// </summary>
    public class CsvExportService
    {
        /// <summary>
        /// Exports a collection of transactions to a CSV file at the specified file path.
        /// The CSV file includes the following columns: Date, Description, Category, Amount, Type.
        /// </summary>
        /// <param name="transactionsToExport">The collection of transactions to write to the CSV file.</param>
        /// <param name="destinationFilePath">The full file path where the CSV file will be saved.</param>
        /// <returns>A task representing the asynchronous file write operation.</returns>
        public async Task ExportTransactionsToCsvAsync(IEnumerable<Transaction> transactionsToExport, string destinationFilePath)
        {
            StringBuilder csvContentBuilder = new StringBuilder();

            csvContentBuilder.AppendLine("Date,Description,Category,Amount,Type");

            foreach (Transaction currentTransaction in transactionsToExport)
            {
                string formattedDate = currentTransaction.Date.ToString("yyyy-MM-dd");
                string sanitizedDescription = EscapeCsvField(currentTransaction.Description);
                string categoryName = currentTransaction.Category != null
                    ? EscapeCsvField(currentTransaction.Category.Name)
                    : "Uncategorized";
                string formattedAmount = currentTransaction.Amount.ToString("F2");
                string transactionTypeName = currentTransaction.Type.ToString();

                string csvLine = $"{formattedDate},{sanitizedDescription},{categoryName},{formattedAmount},{transactionTypeName}";
                csvContentBuilder.AppendLine(csvLine);
            }

            await File.WriteAllTextAsync(destinationFilePath, csvContentBuilder.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Escapes a string value for safe inclusion in a CSV field.
        /// If the value contains commas, quotes, or newlines, it is wrapped in double quotes
        /// and any existing double quotes are doubled.
        /// </summary>
        /// <param name="fieldValue">The raw string value to escape.</param>
        /// <returns>The CSV-safe escaped string.</returns>
        private string EscapeCsvField(string fieldValue)
        {
            if (string.IsNullOrEmpty(fieldValue))
            {
                return string.Empty;
            }

            bool fieldRequiresQuoting = fieldValue.Contains(',')
                || fieldValue.Contains('"')
                || fieldValue.Contains('\n')
                || fieldValue.Contains('\r');

            if (fieldRequiresQuoting)
            {
                string escapedValue = fieldValue.Replace("\"", "\"\"");
                return $"\"{escapedValue}\"";
            }

            return fieldValue;
        }
    }
}
