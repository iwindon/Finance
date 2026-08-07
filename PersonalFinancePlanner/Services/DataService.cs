using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PersonalFinancePlanner.Models;

namespace PersonalFinancePlanner.Services
{
    public class DataService
    {
        private readonly string _dataFilePath;

        public DataService()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appDataFolder, "DebtPayoffCalculator");

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            _dataFilePath = Path.Combine(appFolder, "creditcards.json");
        }

        public List<CreditCard> LoadCreditCards()
        {
            try
            {
                if (!File.Exists(_dataFilePath))
                {
                    return new List<CreditCard>();
                }

                var json = File.ReadAllText(_dataFilePath);
                var cards = JsonSerializer.Deserialize<List<CreditCard>>(json);
                return cards ?? new List<CreditCard>();
            }
            catch (Exception ex)
            {
                // Log or handle error - for now return empty list
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
                return new List<CreditCard>();
            }
        }

        public void SaveCreditCards(IEnumerable<CreditCard> cards)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(cards, options);
                File.WriteAllText(_dataFilePath, json);
            }
            catch (Exception ex)
            {
                // Log or handle error
                System.Diagnostics.Debug.WriteLine($"Error saving data: {ex.Message}");
            }
        }
    }
}
