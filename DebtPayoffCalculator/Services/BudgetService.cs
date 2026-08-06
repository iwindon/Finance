using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DebtPayoffCalculator.Models;

namespace DebtPayoffCalculator.Services
{
    public class BudgetService
    {
        private readonly string _dataFilePath;

        public BudgetService()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appDataFolder, "DebtPayoffCalculator");

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            _dataFilePath = Path.Combine(appFolder, "budget.json");
        }

        public List<BudgetCategory> LoadBudgetCategories()
        {
            try
            {
                if (!File.Exists(_dataFilePath))
                {
                    return GetDefaultCategories();
                }

                var json = File.ReadAllText(_dataFilePath);
                var categories = JsonSerializer.Deserialize<List<BudgetCategory>>(json);
                return categories ?? GetDefaultCategories();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading budget: {ex.Message}");
                return GetDefaultCategories();
            }
        }

        public void SaveBudgetCategories(IEnumerable<BudgetCategory> categories)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(categories, options);
                File.WriteAllText(_dataFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving budget: {ex.Message}");
            }
        }

        private List<BudgetCategory> GetDefaultCategories()
        {
            return new List<BudgetCategory>
            {
                new BudgetCategory { Name = "Housing", BudgetedAmount = 0, IsCustom = false },
                new BudgetCategory { Name = "Transportation", BudgetedAmount = 0, IsCustom = false },
                new BudgetCategory { Name = "Food & Dining", BudgetedAmount = 0, IsCustom = false },
                new BudgetCategory { Name = "Utilities", BudgetedAmount = 0, IsCustom = false },
                new BudgetCategory { Name = "Entertainment", BudgetedAmount = 0, IsCustom = false },
                new BudgetCategory { Name = "Healthcare", BudgetedAmount = 0, IsCustom = false },
                new BudgetCategory { Name = "Personal Care", BudgetedAmount = 0, IsCustom = false },
                new BudgetCategory { Name = "Debt Payments", BudgetedAmount = 0, IsCustom = false },
                new BudgetCategory { Name = "Savings", BudgetedAmount = 0, IsCustom = false },
                new BudgetCategory { Name = "Other", BudgetedAmount = 0, IsCustom = false }
            };
        }
    }
}
