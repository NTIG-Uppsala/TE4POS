using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using static TE4POS.MainWindow;

namespace TE4POS
{
    public static class DatabaseHelper
    {
        private static readonly string dbFilePath = @"..\..\..\..\..\TE4POS\PointOfSale\Databases\Database.db";
        private static readonly string connectionString = @"Data Source=..\..\..\..\..\TE4POS\PointOfSale\Databases\Database.db;Version=3";

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
                        ReceiptNumber INTEGER PRIMARY KEY AUTOINCREMENT,
                        ArticleCount INTEGER NOT NULL,
                        ReceiptTotal INTEGER NOT NULL,
                        Subtotal FLOAT NOT NULL,
                        Saletax FLOAT NOT NULL,
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
                AddProducts(connectionString);
                //AddReceipts();
            }
        }

        public static void AddProducts(string connectionString)
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

        public static void RemoveStock(ObservableCollection<CartItem> allItems)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                foreach (var item in allItems)
                {
                    string query = $"UPDATE Products SET Stock = Stock - '{item.Amount}' WHERE Name = '{item.Name}'";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void AddReceipt(Receipt receipt)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string insertReceiptQuery = @"
                INSERT INTO Receipts (Time, ArticleCount, ReceiptTotal, Subtotal, Saletax)
                VALUES (@time, @articleCount, @receiptTotal, @subtotal, @saletax)";

                using (var cmd = new SQLiteCommand(insertReceiptQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@time", receipt.Time);
                    cmd.Parameters.AddWithValue("@articleCount", receipt.articleCount);
                    cmd.Parameters.AddWithValue("@receiptTotal", receipt.receiptTotal);
                    cmd.Parameters.AddWithValue("@subtotal", receipt.subtotal);
                    cmd.Parameters.AddWithValue("@saletax", receipt.saleTax);

                    cmd.ExecuteNonQuery();

                    // Gets the generated receipt number
                    receipt.receiptNumber = (int)connection.LastInsertRowId;
                }
            }

            // Insert receipt products
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                foreach (var item in receipt.ReceiptProducts)
                {
                    string insertReceiptProductQuery = @"
                    INSERT INTO ReceiptProducts (ReceiptNumber, ProductId, Quantity, UnitPrice, TotalPrice)
                    VALUES (@receiptNumber, @productId, @quantity, @unitPrice, @totalPrice)";
                    using (var cmd = new SQLiteCommand(insertReceiptProductQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@receiptNumber", receipt.receiptNumber);
                        cmd.Parameters.AddWithValue("@productId", GetProductId(item.receiptName));
                        cmd.Parameters.AddWithValue("@quantity", item.receiptAmount);
                        cmd.Parameters.AddWithValue("@unitPrice", item.receiptPrice);
                        cmd.Parameters.AddWithValue("@totalPrice", item.receiptProductTotal);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static int GetProductId(string productName)
        {
            string selectProductIdQuery = "SELECT Id FROM Products WHERE Name = @productName";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(selectProductIdQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@productName", productName);

                    // ExecuteScalar returns the first column of the first row
                    object result = cmd.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int productId))
                    {
                        return productId;
                    }
                    else
                    {
                        throw new Exception("Product not found or invalid Id");
                    }
                }
            }
        }
        public static int GetCurrentReceiptNumber()
        {
            string query = "SELECT MAX(ReceiptNumber) FROM Receipts";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        int temp = Convert.ToInt32(result);
                        return temp;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }
    }
}
