using System;
using System.Collections.ObjectModel;
using WPF_BGPU.Models;

namespace WPF_BGPU.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<Transaction> Transactions { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ObservableCollection<string> Categories { get; set; }
        public string SelectedCategory { get; set; }

        public MainViewModel()
        {
            Categories = new ObservableCollection<string>
            {
                "Продукты",
                "Транспорт",
                "Развлечения"
            };

            Transactions = new ObservableCollection<Transaction>
            {
                new Transaction
                {
                    Date = DateTime.Today,
                    Category = "Продукты",
                    Amount = 350,
                    Description = "Покупки в магазине"
                },
                new Transaction
                {
                    Date = DateTime.Today.AddDays(-1),
                    Category = "Транспорт",
                    Amount = 120,
                    Description = "Проезд"
                }
            };
        }
    }
}
