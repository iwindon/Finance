using PersonalFinancePlanner.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PersonalFinancePlanner.Services
{
    public class DebtPlanService
    {
        private readonly string _dataFolder;
        private readonly string _plansFolder;

        public DebtPlanService()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _dataFolder = Path.Combine(appDataFolder, "DebtPayoffCalculator");
            _plansFolder = Path.Combine(_dataFolder, "Plans");

            if (!Directory.Exists(_plansFolder))
            {
                Directory.CreateDirectory(_plansFolder);
            }
        }

        public List<DebtPlan> LoadAllPlans()
        {
            var plans = new List<DebtPlan>();

            if (!Directory.Exists(_plansFolder))
                return plans;

            var planFiles = Directory.GetFiles(_plansFolder, "*.json");

            foreach (var file in planFiles)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var plan = JsonSerializer.Deserialize<DebtPlan>(json);
                    if (plan != null)
                    {
                        plans.Add(plan);
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue loading other plans
                    Console.WriteLine($"Error loading plan {file}: {ex.Message}");
                }
            }

            return plans.OrderByDescending(p => p.LastModifiedDate).ToList();
        }

        public DebtPlan? LoadPlan(string planName)
        {
            var fileName = GetSafeFileName(planName);
            var filePath = Path.Combine(_plansFolder, $"{fileName}.json");

            if (!File.Exists(filePath))
                return null;

            try
            {
                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<DebtPlan>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading plan {planName}: {ex.Message}");
                return null;
            }
        }

        public void SavePlan(DebtPlan plan)
        {
            if (plan == null)
                throw new ArgumentNullException(nameof(plan));

            if (string.IsNullOrWhiteSpace(plan.Name))
                throw new ArgumentException("Plan name cannot be empty");

            plan.LastModifiedDate = DateTime.Now;

            var fileName = GetSafeFileName(plan.Name);
            var filePath = Path.Combine(_plansFolder, $"{fileName}.json");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(plan, options);
            File.WriteAllText(filePath, json);
        }

        public void DeletePlan(string planName)
        {
            var fileName = GetSafeFileName(planName);
            var filePath = Path.Combine(_plansFolder, $"{fileName}.json");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public bool PlanExists(string planName)
        {
            var fileName = GetSafeFileName(planName);
            var filePath = Path.Combine(_plansFolder, $"{fileName}.json");
            return File.Exists(filePath);
        }

        private string GetSafeFileName(string planName)
        {
            // Remove invalid file name characters
            var invalidChars = Path.GetInvalidFileNameChars();
            var safeName = string.Join("_", planName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
            return safeName;
        }
    }
}
