using System.Collections.ObjectModel;
using System.Data.SQLite;
using static TE4POS.MainWindow;

namespace ProductsRepository
{
    public interface IProductsRepository
    {
        IEnumerable<Product> GetAllProducts();
    }

    public class SQLiteProductsRepository : IProductsRepository
    {
        public ObservableCollection<Product> AllProducts { get; private set; }
            = new ObservableCollection<Product>();

        private static readonly string filePath = "Databases/Database.db";
        private static readonly string connectionString = @"Data Source=" + filePath + ";Version=3";

        public IEnumerable<Product> GetAllProducts()
        {
            AllProducts.Clear(); // important might cause duplication otherwise

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
                            id = reader.GetInt32(reader.GetOrdinal("Id")),
                            name = reader.GetString(reader.GetOrdinal("Name")),
                            price = reader.GetInt32(reader.GetOrdinal("Price")),
                            category = reader.GetString(reader.GetOrdinal("Category")),
                            stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                        });
                    }
                }
            }
            return AllProducts;
        }
    }
}
