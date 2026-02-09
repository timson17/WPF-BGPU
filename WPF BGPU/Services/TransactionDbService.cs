using System.Collections.Generic;
using Microsoft.Data.SqlClient;

using WPF_BGPU.Models;

namespace WPF_BGPU.Services
{
    public class TransactionDbService
    {
        private readonly string _connectionString;

        public TransactionDbService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Transaction> GetAll()
        {
            var list = new List<Transaction>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(
                "SELECT Id, Date, Category, Amount, Description FROM Transactions ORDER BY Date DESC",
                conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Transaction
                {
                    Id = reader.GetInt32(0),
                    Date = reader.GetDateTime(1),
                    Category = reader.GetString(2),
                    Amount = reader.GetDecimal(3),
                    Description = reader.IsDBNull(4) ? "" : reader.GetString(4)
                });
            }

            return list;
        }

        public void Add(Transaction transaction)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(
                @"INSERT INTO Transactions (Date, Category, Amount, Description)
                  VALUES (@Date, @Category, @Amount, @Description)", conn);

            cmd.Parameters.AddWithValue("@Date", transaction.Date);
            cmd.Parameters.AddWithValue("@Category", transaction.Category);
            cmd.Parameters.AddWithValue("@Amount", transaction.Amount);
            cmd.Parameters.AddWithValue("@Description", transaction.Description);

            cmd.ExecuteNonQuery();
        }
    }
}
