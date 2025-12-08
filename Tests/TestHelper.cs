using System.Data.SQLite;


namespace Tests
{
    internal static class TestHelper
    {
        private static readonly string filePath = "Databases/TestDatabase.db";
        private static readonly string connectionString = @"Data Source=" + filePath + ";Version=3";

        public static void InitializeTestDatabase()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            SQLiteConnection.CreateFile(filePath);
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Creates the database tables
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
                    ReceiptNumber INTEGER PRIMARY KEY AUTOINCREMENT,
                    ArticleCount INTEGER NOT NULL,
                    ReceiptTotal INTEGER NOT NULL,
                    Subtotal FLOAT NOT NULL,
                    Saletax FLOAT NOT NULL,
                    PdfFormattedTime TEXT NOT NULL,
                    Time TEXT NOT NULL
                )";

                string createReceiptProductsQuery = @"
                CREATE TABLE IF NOT EXISTS ReceiptProducts(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ReceiptNumber INTEGER NOT NULL,
                    ProductId INTEGER NOT NULL,
                    Quantity INTEGER NOT NULL,
                    UnitPrice INTEGER NOT NULL,
                    TotalPrice INTEGER NOT NULL,
                    FOREIGN KEY (ReceiptNumber) REFERENCES Receipts(ReceiptNumber),
                    FOREIGN KEY (ProductId) REFERENCES Products(Id)
                )";

                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = createProductsQuery;
                    command.ExecuteNonQuery();

                    command.CommandText = createReceiptsQuery;
                    command.ExecuteNonQuery();

                    command.CommandText = createReceiptProductsQuery;
                    command.ExecuteNonQuery();
                }
            }

            // Inserts test data into the Products table
            var products = new[]
            {
                new { Name = "Testobject", Price = 30, Category = "Varma drycker", Stock = 100 },
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

                    foreach (var product in products)
                    {
                        cmd.Parameters["@name"].Value = product.Name;
                        cmd.Parameters["@price"].Value = product.Price;
                        cmd.Parameters["@category"].Value = product.Category;
                        cmd.Parameters["@stock"].Value = product.Stock;
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
            }
        }

        public static void DeleteTestDatabase()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
