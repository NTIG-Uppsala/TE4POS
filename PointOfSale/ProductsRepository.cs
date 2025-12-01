using System.Collections.ObjectModel;
using System.Data.SQLite;
using TE4POS;

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

        private string connectionString = @"Data Source=..\..\..\..\..\TE4POS\PointOfSale\Databases\Database.db;Version=3";

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
