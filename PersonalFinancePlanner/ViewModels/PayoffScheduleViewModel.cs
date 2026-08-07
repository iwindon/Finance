using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using PersonalFinancePlanner.Models;

namespace PersonalFinancePlanner.ViewModels
{
    public class PayoffScheduleViewModel : INotifyPropertyChanged
    {
        private string _payoffMethod = string.Empty;
        private decimal _extraPayment;
        private decimal _totalInterest;
        private decimal _totalPaid;
        private int _totalMonths;

        public ObservableCollection<PayoffScheduleEntry> ScheduleEntries { get; set; }
        public ObservableCollection<string> CardNames { get; set; }

        public string PayoffMethod
        {
            get => _payoffMethod;
            set { _payoffMethod = value; OnPropertyChanged(); }
        }

        public decimal ExtraPayment
        {
            get => _extraPayment;
            set { _extraPayment = value; OnPropertyChanged(); }
        }

        public decimal TotalInterest
        {
            get => _totalInterest;
            set { _totalInterest = value; OnPropertyChanged(); }
        }

        public decimal TotalPaid
        {
            get => _totalPaid;
            set { _totalPaid = value; OnPropertyChanged(); }
        }

        public int TotalMonths
        {
            get => _totalMonths;
            set { _totalMonths = value; OnPropertyChanged(); }
        }

        public string PayoffTimeString
        {
            get
            {
                int years = TotalMonths / 12;
                int months = TotalMonths % 12;
                return years > 0
                    ? $"{years} year(s) and {months} month(s)"
                    : $"{months} month(s)";
            }
        }

        public PayoffScheduleViewModel(List<PayoffScheduleEntry> schedule, string method, decimal extra, decimal interest, decimal paid, int months)
        {
            ScheduleEntries = new ObservableCollection<PayoffScheduleEntry>(schedule);
            CardNames = new ObservableCollection<string>(schedule.Select(e => e.CardName).Distinct().OrderBy(n => n));
            PayoffMethod = method;
            ExtraPayment = extra;
            TotalInterest = interest;
            TotalPaid = paid;
            TotalMonths = months;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
