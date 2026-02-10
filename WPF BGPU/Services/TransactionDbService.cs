using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using WPF_BGPU.Models;

namespace WPF_BGPU.Services
{
    public class TransactionDbService
    {
        private readonly string _connectionString;
        private readonly CategoryDbService _categoryDb;
        private readonly AccountDbService _accountDb;

        public TransactionDbService(string connectionString)
        {
            _connectionString = connectionString;
            _categoryDb = new CategoryDbService(connectionString);
            _accountDb = new AccountDbService(connectionString);
        }

        public List<Transaction> GetAll()
        {
            var list = new List<Transaction>();
            var categories = _categoryDb.GetAll();
            var accounts = _accountDb.GetAll();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT Id, Date, Category, Amount, Description, AccountId FROM Transactions", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string categoryName = reader.GetString(2);
                var category = categories.Find(c => c.Name == categoryName) ?? new Category { Name = categoryName };

                int accountId = reader.GetInt32(5);
                var account = accounts.Find(a => a.Id == accountId) ?? new Account { Id = accountId, Name = "Неизвестно" };

                list.Add(new Transaction
                {
                    Id = reader.GetInt32(0),
                    Date = reader.GetDateTime(1),
                    Category = category,
                    Amount = reader.GetDecimal(3),
                    Description = reader.GetString(4),
                    Account = account
                });
            }

            return list;
        }

        public void Add(Transaction t)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(
                "INSERT INTO Transactions (Date, Category, Amount, Description, AccountId) VALUES (@date, @category, @amount, @description, @accountId)",
                conn);
            cmd.Parameters.AddWithValue("@date", t.Date);
            cmd.Parameters.AddWithValue("@category", t.Category.Name);
            cmd.Parameters.AddWithValue("@amount", t.Amount);
            cmd.Parameters.AddWithValue("@description", t.Description ?? "");
            cmd.Parameters.AddWithValue("@accountId", t.Account.Id);
            cmd.ExecuteNonQuery();
        }
    }
}
