using System;

namespace WPF_BGPU.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Category Category { get; set; } = new Category();
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public Account Account { get; set; } = new Account();
    }
}
