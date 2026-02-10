using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using WPF_BGPU.Models;

namespace WPF_BGPU.Services
{
    public class AccountDbService
    {
        private readonly string _connectionString;

        public AccountDbService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Account> GetAll()
        {
            var list = new List<Account>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT Id, Name, Balance FROM Accounts", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Account
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Balance = reader.GetDecimal(2)
                });
            }
            return list;
        }
    }
}
