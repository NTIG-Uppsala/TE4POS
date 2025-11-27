using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace TE4POS
{
    public interface IProductsRepository
    {
        IEnumerable<Product> GetAllProducts();
    }

    public class SQLiteProductsRepository : IProductsRepository
    {
        public ObservableCollection<Product> AllProducts { get; private set; }
            = new ObservableCollection<Product>();

        private string connectionString = @"Data Source=..\..\..\..\..\TE4POS\PointOfSale\database.db;Version=3";

        public IEnumerable<Product> GetAllProducts()
        {
            AllProducts.Clear(); // important to avoid duplicates

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Products";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AllProducts.Add(new Product()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Price = reader.GetInt32(reader.GetOrdinal("Price")),
                            Category = reader.GetString(reader.GetOrdinal("Category")),
                            Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                        });
                    }
                }
            }

            return AllProducts;
        }
    }
}
