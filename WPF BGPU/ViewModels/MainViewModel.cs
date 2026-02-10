using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WPF_BGPU.Models;
using WPF_BGPU.Services;

namespace WPF_BGPU.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly TransactionDbService _transactionDb;
        private readonly AccountDbService _accountDb;
        private readonly CategoryDbService _categoryDb;

        public ObservableCollection<Transaction> Transactions { get; set; }
        public ObservableCollection<Account> Accounts { get; set; }
        public ObservableCollection<Category> Categories { get; set; }

        public string NewCategoryName { get; set; }
        public Category NewCategory { get; set; }
        public Account NewAccount { get; set; }
        public DateTime NewDate { get; set; } = DateTime.Today;
        public decimal NewAmount { get; set; }
        public string NewDescription { get; set; }

        public DateTime? FilterStartDate { get; set; }
        public DateTime? FilterEndDate { get; set; }
        public Category FilterCategory { get; set; }
        public Account FilterAccount { get; set; }

        public decimal TotalAmount => Transactions.Sum(t => t.Amount);

        // Команды
        public ICommand AddTransactionCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand ApplyFilterCommand { get; }
        public ICommand ResetFilterCommand { get; }
        public ICommand ExportCsvCommand { get; }
        public ICommand ExportPdfCommand { get; }

        public MainViewModel()
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            _transactionDb = new TransactionDbService(cs);
            _accountDb = new AccountDbService(cs);
            _categoryDb = new CategoryDbService(cs);

            Transactions = new ObservableCollection<Transaction>();
            Accounts = new ObservableCollection<Account>(_accountDb.GetAll());
            Categories = new ObservableCollection<Category>(_categoryDb.GetAll());

            AddTransactionCommand = new RelayCommand(_ => AddTransaction());
            AddCategoryCommand = new RelayCommand(_ => AddCategory());
            ApplyFilterCommand = new RelayCommand(_ => ApplyFilter());
            ResetFilterCommand = new RelayCommand(_ => ResetFilter());
            ExportCsvCommand = new RelayCommand(_ => ExportCsv());
            ExportPdfCommand = new RelayCommand(_ => ExportPdf());

            LoadTransactions();
        }

        private void LoadTransactions()
        {
            Transactions.Clear();
            var list = _transactionDb.GetAll();

            if (FilterStartDate.HasValue)
                list = list.Where(t => t.Date >= FilterStartDate.Value).ToList();
            if (FilterEndDate.HasValue)
                list = list.Where(t => t.Date <= FilterEndDate.Value).ToList();
            if (FilterCategory != null)
                list = list.Where(t => t.Category?.Id == FilterCategory.Id).ToList();
            if (FilterAccount != null)
                list = list.Where(t => t.Account?.Id == FilterAccount.Id).ToList();

            foreach (var t in list)
                Transactions.Add(t);

            OnPropertyChanged(nameof(TotalAmount));
        }

        private void AddTransaction()
        {
            if (NewAccount == null || NewCategory == null)
            {
                MessageBox.Show("Выберите счёт и категорию!");
                return;
            }

            var t = new Transaction
            {
                Date = NewDate,
                Account = NewAccount,
                Category = NewCategory,
                Amount = NewAmount,
                Description = NewDescription
            };

            _transactionDb.Add(t);
            LoadTransactions();
        }

        private void AddCategory()
        {
            if (!string.IsNullOrWhiteSpace(NewCategoryName))
            {
                var category = new Category { Name = NewCategoryName };
                _categoryDb.Add(category);       // Добавляем в базу и получаем Id
                Categories.Add(category);        // Добавляем в ObservableCollection
                NewCategoryName = string.Empty;
                OnPropertyChanged(nameof(NewCategoryName));
            }
        }

        private void ApplyFilter()
        {
            LoadTransactions();
        }

        private void ResetFilter()
        {
            FilterStartDate = null;
            FilterEndDate = null;
            FilterCategory = null;
            FilterAccount = null;
            OnPropertyChanged(nameof(FilterStartDate));
            OnPropertyChanged(nameof(FilterEndDate));
            OnPropertyChanged(nameof(FilterCategory));
            OnPropertyChanged(nameof(FilterAccount));
            LoadTransactions();
        }

        private void ExportCsv()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "report.csv"
            };

            if (saveFileDialog.ShowDialog() != true) return;

            using var sw = new StreamWriter(saveFileDialog.FileName);
            sw.WriteLine("Дата,Счёт,Категория,Сумма,Описание");

            foreach (var t in Transactions)
                sw.WriteLine($"{t.Date:d},{t.Account?.Name},{t.Category?.Name},{t.Amount},{t.Description}");

            MessageBox.Show($"CSV сохранён: {saveFileDialog.FileName}", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportPdf()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*",
                FileName = "report.pdf"
            };

            if (saveFileDialog.ShowDialog() != true) return;

            var path = saveFileDialog.FileName;
            var doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
            doc.Open();

            string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");
            var bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var font = new Font(bf, 12);

            var table = new PdfPTable(5) { WidthPercentage = 100 };
            table.AddCell(new PdfPCell(new Phrase("Дата", font)));
            table.AddCell(new PdfPCell(new Phrase("Счёт", font)));
            table.AddCell(new PdfPCell(new Phrase("Категория", font)));
            table.AddCell(new PdfPCell(new Phrase("Сумма", font)));
            table.AddCell(new PdfPCell(new Phrase("Описание", font)));

            foreach (var t in Transactions)
            {
                table.AddCell(new PdfPCell(new Phrase(t.Date.ToShortDateString(), font)));
                table.AddCell(new PdfPCell(new Phrase(t.Account?.Name ?? "", font)));
                table.AddCell(new PdfPCell(new Phrase(t.Category?.Name ?? "", font)));
                table.AddCell(new PdfPCell(new Phrase(t.Amount.ToString("N2"), font)));
                table.AddCell(new PdfPCell(new Phrase(t.Description ?? "", font)));
            }

            doc.Add(table);
            doc.Close();

            MessageBox.Show($"PDF сохранён: {path}", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
