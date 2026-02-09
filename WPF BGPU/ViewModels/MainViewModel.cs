using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows.Input;
using WPF_BGPU.Models;
using WPF_BGPU.Services;

namespace WPF_BGPU.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly TransactionDbService _db;

        // ===== ТАБЛИЦА =====
        public ObservableCollection<Transaction> Transactions { get; set; }

        // ===== КАТЕГОРИИ =====
        public ObservableCollection<string> Categories { get; set; }

        // ===== ФИЛЬТР =====
        private DateTime? _filterStartDate;
        public DateTime? FilterStartDate
        {
            get => _filterStartDate;
            set { _filterStartDate = value; OnPropertyChanged(); }
        }

        private DateTime? _filterEndDate;
        public DateTime? FilterEndDate
        {
            get => _filterEndDate;
            set { _filterEndDate = value; OnPropertyChanged(); }
        }

        private string _filterCategory;
        public string FilterCategory
        {
            get => _filterCategory;
            set { _filterCategory = value; OnPropertyChanged(); }
        }

        // ===== НОВАЯ ОПЕРАЦИЯ =====
        private DateTime _newDate = DateTime.Today;
        public DateTime NewDate
        {
            get => _newDate;
            set { _newDate = value; OnPropertyChanged(); }
        }

        private string _newCategory;
        public string NewCategory
        {
            get => _newCategory;
            set { _newCategory = value; OnPropertyChanged(); }
        }

        private decimal _newAmount;
        public decimal NewAmount
        {
            get => _newAmount;
            set { _newAmount = value; OnPropertyChanged(); }
        }

        private string _newDescription;
        public string NewDescription
        {
            get => _newDescription;
            set { _newDescription = value; OnPropertyChanged(); }
        }

        // ===== КОМАНДЫ =====
        public ICommand AddTransactionCommand { get; }

        public MainViewModel()
        {
            string cs = ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

            _db = new TransactionDbService(cs);

            Categories = new ObservableCollection<string>
            {
                "Продукты",
                "Транспорт",
                "Развлечения"
            };

            Transactions = new ObservableCollection<Transaction>();

            AddTransactionCommand = new RelayCommand(AddTransaction);

            LoadTransactions();
        }

        private void LoadTransactions()
        {
            Transactions.Clear();
            foreach (var t in _db.GetAll())
                Transactions.Add(t);
        }

        private void AddTransaction(object obj)
        {
            if (NewCategory == null || NewAmount <= 0)
                return;

            var transaction = new Transaction
            {
                Date = NewDate,
                Category = NewCategory,
                Amount = NewAmount,
                Description = NewDescription
            };

            _db.Add(transaction);
            LoadTransactions();

            // очистка формы
            NewAmount = 0;
            NewDescription = string.Empty;
        }
    }
}
