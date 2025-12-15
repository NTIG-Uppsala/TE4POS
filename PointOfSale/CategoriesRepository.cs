using System.Collections.ObjectModel;
using System.Data.SQLite;
using static TE4POS.MainWindow;
using TE4POS;

namespace CategoriesRepository
{
    public interface ICategoriesRepository
    {
        IEnumerable<Category> GetAllCategories();
    }

    public class SQLiteCategoiesRepository : ICategoriesRepository
    {
        private static readonly string filePath = "Databases/Database.db";
        private static readonly string connectionString = @"Data Source=" + filePath + ";Version=3";

        private static readonly string testFilePath = "Databases/TestDatabase.db";
        private static readonly string testConnectionString = @"Data Source=" + testFilePath + ";Version=3";

        private static string currentConnectionString = "";

        public ObservableCollection<Category> AllCategories { get; private set; }
            = new ObservableCollection<Category>();

        public IEnumerable<Category> GetAllCategories()
        {
            if (App.isTest)
            {
                currentConnectionString = testConnectionString;
            }
            else
            {
                currentConnectionString = connectionString;
            }
            AllCategories.Clear(); // important might cause duplication otherwise

            using (var connection = new SQLiteConnection(currentConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM ProductCategories";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AllCategories.Add(new Category()
                        {
                            id = reader.GetInt32(reader.GetOrdinal("Id")),
                            name = reader.GetString(reader.GetOrdinal("CategoryName")),
                        });
                    }
                }
            }
            return AllCategories;
        }
    }
}
