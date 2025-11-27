using System.Data.SQLite;
using System.IO;

namespace TE4POS
{
    public static class DatabaseHelper
    {
        private static readonly string connectionString = @"Data Source=..\..\..\..\..\TE4POS\PointOfSale\database.db;Version=3";
        private static readonly string dbFilePath = @"..\..\..\..\..\TE4POS\PointOfSale\database.db";
        public static void InitializeDatabase()
        {
            if (!File.Exists(dbFilePath))
            {
                SQLiteConnection.CreateFile(dbFilePath);
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Creates the tables
                    string createProductsQuery = @"
                    CREATE TABLE IF NOT EXISTS Products (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Price INTEGER NOT NULL,
                        Category TEXT NOT NULL,
                        Stock INTEGER NOT NULL
                    )";

                    string createReceiptsQuery = @"
                    CREATE TABLE IF NOT EXISTS Receipts (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Price REAL NOT NULL
                    )";

                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = createProductsQuery;
                        command.ExecuteNonQuery();

                        command.CommandText = createReceiptsQuery;
                        command.ExecuteNonQuery();
                    }
                }
                AddProducts();
            }
        }

        public static void AddProducts()
        {
            var products = new[]
            {
                new { Name = "Bryggkaffe (liten)", Price = 28, Category = "Varma drycker", Stock = 100 },
                new { Name = "Bryggkaffe (stor)", Price = 34, Category = "Varma drycker", Stock = 100 },
                new { Name = "Cappuccino", Price = 42, Category = "Varma drycker", Stock = 100 },
                new { Name = "Latte", Price = 46, Category = "Varma drycker", Stock = 100 },
                new { Name = "Varm choklad med grädde", Price = 45, Category = "Varma drycker", Stock = 100 },
                new { Name = "Te (svart, grönt eller örtte)", Price = 32, Category = "Varma drycker", Stock = 100 },

                new { Name = "Islatte", Price = 48, Category = "Kalla drycker", Stock = 100 },
                new { Name = "Ischai", Price = 46, Category = "Kalla drycker", Stock = 100 },
                new { Name = "Läsk (33 cl)", Price = 22, Category = "Kalla drycker", Stock = 100 },
                new { Name = "Mineralvatten", Price = 20, Category = "Kalla drycker", Stock = 100 },
                new { Name = "Smoothie (jordgubb & banan)", Price = 55, Category = "Kalla drycker", Stock = 100 },
                new { Name = "Färskpressad apelsinjuice", Price = 49, Category = "Kalla drycker", Stock = 100 },

                new { Name = "Kanelbulle", Price = 25, Category = "Bakverk", Stock = 100 },
                new { Name = "Chokladboll", Price = 18, Category = "Bakverk", Stock = 100 },
                new { Name = "Morotskaka (bit)", Price = 38, Category = "Bakverk", Stock = 100 },
                new { Name = "Cheesecake (bit)", Price = 42, Category = "Bakverk", Stock = 100 },
                new { Name = "Croissant", Price = 26, Category = "Bakverk", Stock = 100 },
                new { Name = "Muffins (blåbär)", Price = 28, Category = "Bakverk", Stock = 100 },

                new { Name = "Smörgås (ost & skinka)", Price = 38, Category = "Enkel mat", Stock = 100 },
                new { Name = "Räksmörgås", Price = 69, Category = "Enkel mat", Stock = 100 },
                new { Name = "Panini (kyckling & pesto)", Price = 58, Category = "Enkel mat", Stock = 100 },
                new { Name = "Soppa med bröd", Price = 65, Category = "Enkel mat", Stock = 100 },
                new { Name = "Quinoasallad", Price = 72, Category = "Enkel mat", Stock = 100 },
            };

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                using (var cmd = new SQLiteCommand(@"INSERT INTO Products (Name, Price, Category, Stock) 
                                           VALUES (@name, @price, @category, @stock)", connection, tx))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@name"));
                    cmd.Parameters.Add(new SQLiteParameter("@price"));
                    cmd.Parameters.Add(new SQLiteParameter("@category"));
                    cmd.Parameters.Add(new SQLiteParameter("@stock"));

                    foreach (var p in products)
                    {
                        cmd.Parameters["@name"].Value = p.Name;
                        cmd.Parameters["@price"].Value = p.Price;
                        cmd.Parameters["@category"].Value = p.Category;
                        cmd.Parameters["@stock"].Value = p.Stock;
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();

                    //connection.Open();
                    //string insertProductQuery = @"
                    //INSERT INTO Products (Name, Price, Category, Stock) VALUES
                    //('Bryggkaffe (liten)', 28, Varma_drycker, 100),
                    //('Bryggkaffe (stor)', 34, Varma_drycker, 100),
                    //('Cappuccino', 42, Varma_drycker, 100),
                    //('Latte', 46, Varma_drycker, 100),
                    //('Varm choklad med grädde', 45, Varma_drycker, 100),
                    //('Te (svart, grönt eller örtte)', 32, Varma_drycker, 100),

                    //('Islatte', 48, Kalla_drycker, 100)
                    //('Ischai', 46, Kalla_drycker, 100)
                    //('Läsk (33cl)', 22, Kalla_drycker, 100)
                    //('Mineralvatten', 20, Kalla_drycker, 100)
                    //('Smoothie (jordgubb & banan)', 55, Kalla_drycker, 100)
                    //('Färskpressad apelsinjuice', 49, Kalla_drycker, 100)

                    //('Kanelbulle', 25, Bakverk, 100),
                    //('Chokladboll', 18, Bakverk, 100),
                    //('Morotskaka (bit)', 38, Bakverk, 100),
                    //('Cheesecake (bit)', 42, Bakverk, 100),
                    //('Crossiant', 26, Bakverk, 100),
                    //('Muffins (blåbär)', 28, Bakverk, 100),

                    //('Smörgås med (ost och skinka)', 38, Enkel_mat, 100),
                    //('Räksmörgås', 69, Enkel_mat, 100),
                    //('Panini (kyckling & pesto)', 58, Enkel_mat, 100),
                    //('Soppa med bröd', 65, Enkel_mat, 100),
                    //('Quinoasallad', 72, Enkel_mat, 100);
                    //";
                    //using (SQLiteCommand command = new SQLiteCommand(insertProductQuery, connection))
                    //{
                    //    command.ExecuteNonQuery();
                    //}
                }
            }
        }
    }
}
