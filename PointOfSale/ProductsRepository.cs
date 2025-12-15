using System.Collections.ObjectModel;
using System.Data.SQLite;
using static TE4POS.MainWindow;
using TE4POS;

namespace ProductsRepository
{
    public interface IProductsRepository
    {
        IEnumerable<Product> GetAllProducts();
    }

    public class SQLiteProductsRepository : IProductsRepository
    {
        private static readonly string filePath = "Databases/Database.db";
        private static readonly string connectionString = @"Data Source=" + filePath + ";Version=3";

        private static readonly string testFilePath = "Databases/TestDatabase.db";
        private static readonly string testConnectionString = @"Data Source=" + testFilePath + ";Version=3";

        private static string currentConnectionString = "";

        public ObservableCollection<Product> AllProducts { get; private set; }
            = new ObservableCollection<Product>();

        public IEnumerable<Product> GetAllProducts()
        {
            if (App.isTest)
            {
                currentConnectionString = testConnectionString;
            }
            else
            {
                currentConnectionString = connectionString;
            }
            AllProducts.Clear(); // important might cause duplication otherwise

            using (var connection = new SQLiteConnection(currentConnectionString))
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
                            id = reader.GetInt32(reader.GetOrdinal("Id")),
                            name = reader.GetString(reader.GetOrdinal("Name")),
                            price = reader.GetInt32(reader.GetOrdinal("Price")),
                            category = reader.GetInt32(reader.GetOrdinal("Category")),
                            sold = reader.GetInt32(reader.GetOrdinal("Sold")),
                        });
                    }
                }
            }
            return AllProducts;
        }
    }
}
