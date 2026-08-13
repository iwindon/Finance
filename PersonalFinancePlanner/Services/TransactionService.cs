using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PersonalFinancePlanner.Models;

namespace PersonalFinancePlanner.Services
{
    public class TransactionService
    {
        private readonly string _dataFilePath;

        public TransactionService()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appDataFolder, "DebtPayoffCalculator");

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            _dataFilePath = Path.Combine(appFolder, "transactions.json");
        }

        public List<Transaction> LoadTransactions()
        {
            try
            {
                if (!File.Exists(_dataFilePath))
                {
                    return new List<Transaction>();
                }

                var json = File.ReadAllText(_dataFilePath);
                var transactions = JsonSerializer.Deserialize<List<Transaction>>(json);
                return transactions ?? new List<Transaction>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading transactions: {ex.Message}");
                return new List<Transaction>();
            }
        }

        public void SaveTransactions(IEnumerable<Transaction> transactions)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(transactions.OrderByDescending(t => t.Date), options);
                File.WriteAllText(_dataFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving transactions: {ex.Message}");
            }
        }

        /// <summary>
        /// Fixes categories for existing transactions to match their type
        /// Converts DebtPayment type to Expense type with "Debt Payments" category
        /// </summary>
        public void MigrateTransactionCategories()
        {
            var transactions = LoadTransactions();
            bool modified = false;

            foreach (var transaction in transactions)
            {
                // Convert old DebtPayment type to Expense with "Debt Payments" category
                if (transaction.Type == TransactionType.DebtPayment)
                {
                    transaction.Type = TransactionType.Expense;
                    transaction.Category = "Debt Payments";
                    modified = true;
                }
                // Ensure Savings transactions have "Savings" category
                else if (transaction.Type == TransactionType.Savings && transaction.Category != "Savings")
                {
                    transaction.Category = "Savings";
                    modified = true;
                }
            }

            if (modified)
            {
                SaveTransactions(transactions);
            }
        }
    }
}
