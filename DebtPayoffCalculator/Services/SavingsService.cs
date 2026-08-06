using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DebtPayoffCalculator.Models;

namespace DebtPayoffCalculator.Services
{
    public class SavingsService
    {
        private readonly string _dataFilePath;

        public SavingsService()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appDataFolder, "DebtPayoffCalculator");

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            _dataFilePath = Path.Combine(appFolder, "savings.json");
        }

        public List<SavingsEntry> LoadSavingsEntries()
        {
            try
            {
                if (!File.Exists(_dataFilePath))
                {
                    return new List<SavingsEntry>();
                }

                var json = File.ReadAllText(_dataFilePath);
                var entries = JsonSerializer.Deserialize<List<SavingsEntry>>(json);
                return entries ?? new List<SavingsEntry>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading savings: {ex.Message}");
                return new List<SavingsEntry>();
            }
        }

        public void SaveSavingsEntries(IEnumerable<SavingsEntry> entries)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(entries.OrderByDescending(e => e.Date), options);
                File.WriteAllText(_dataFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving savings: {ex.Message}");
            }
        }
    }
}
