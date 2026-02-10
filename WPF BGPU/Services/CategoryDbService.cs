using Microsoft.Data.SqlClient;
using WPF_BGPU.Models;
using System.Collections.Generic;

namespace WPF_BGPU.Services
{
    public class CategoryDbService
    {
        private readonly string _connectionString;

        public CategoryDbService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Category> GetAll()
        {
            var list = new List<Category>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT Id, Name FROM Categories ORDER BY Name", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Category
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }

            return list;
        }

        public void Add(Category category)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("INSERT INTO Categories (Name) VALUES (@name); SELECT SCOPE_IDENTITY();", conn);
            cmd.Parameters.AddWithValue("@name", category.Name);

            // Получаем Id, чтобы сохранить в объекте
            var id = cmd.ExecuteScalar();
            category.Id = Convert.ToInt32(id);
        }
    }
}
