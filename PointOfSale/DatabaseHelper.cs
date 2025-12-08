using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using static TE4POS.MainWindow;

namespace TE4POS
{
    public static class DatabaseHelper
    {

        private static readonly string filePath = "Databases/Database.db";
        private static readonly string connectionString = @"Data Source=" + filePath + ";Version=3";

        private static readonly string testFilePath = "Databases/TestDatabase.db";
        private static readonly string testConnectionString = @"Data Source=" + testFilePath + ";Version=3";

        public static string currentConnectionString = "";

        public static string GetConnectionString()
        {
            if (App.isTest)
            {
                currentConnectionString = testConnectionString;
            }
            else
            {
                currentConnectionString = connectionString;
            }
            return currentConnectionString;
        }
        public static void InitializeDatabase()
        {
            if (App.isTest)
            {
                return;
            }
            else
            {
                if (!File.Exists(filePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    SQLiteConnection.CreateFile(filePath);
                    using (var connection = new SQLiteConnection(GetConnectionString()))
                    {
                        connection.Open();

                        // Creates the database tables
                        string createProductsQuery = @"
                    CREATE TABLE IF NOT EXISTS Products (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Price INTEGER NOT NULL,
                        Category TEXT NOT NULL,
                        Sold INTEGER NOT NULL
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

                         string createCategoriesQuery = @"
                    CREATE TABLE IF NOT EXISTS ProductCategories(
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        CategoryName TEXT NOT NULL
                    )";

                        using (var command = new SQLiteCommand(connection))
                        {
                            command.CommandText = createProductsQuery;
                            command.ExecuteNonQuery();

                            command.CommandText = createReceiptsQuery;
                            command.ExecuteNonQuery();

                            command.CommandText = createReceiptProductsQuery;
                            command.ExecuteNonQuery();

                            command.CommandText = createCategoriesQuery;
                            command.ExecuteNonQuery();
                        }
                    }
                    AddCategories(GetConnectionString());
                    AddProducts(GetConnectionString());
                }
            }
        }

        public static void AddCategories(string connectionString)
        {
            var Categories = new[]
            {
                new{Id = 1, CategoryName = "Varma drycker"},
                new{Id = 2, CategoryName = "Kalla drycker"},
                new{Id = 3, CategoryName = "Bakverk"},
                new{Id = 4, CategoryName = "Enkel mat"}
            };
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                using (var cmd = new SQLiteCommand(@"INSERT INTO ProductCategories (Id, CategoryName) 
                                           VALUES (@id, @category)", connection, tx))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@id"));
                    cmd.Parameters.Add(new SQLiteParameter("@category"));

                    foreach (var category in Categories)
                    {
                        cmd.Parameters["@id"].Value = category.Id;
                        cmd.Parameters["@category"].Value = category.CategoryName;
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
            }
        }

        public static void AddProducts(string connectionString)
        {
            var products = new[]
            {
                new { Name = "Bryggkaffe (liten)", Price = 28, Category = "1", Sold = 0 },
                new { Name = "Bryggkaffe (stor)", Price = 34, Category = "1", Sold = 0 },
                new { Name = "Cappuccino", Price = 42, Category = "1", Sold = 0 },
                new { Name = "Latte", Price = 46, Category = "1", Sold = 0 },
                new { Name = "Varm choklad med grädde", Price = 45, Category = "1", Sold = 0 },
                new { Name = "Te (svart, grönt eller örtte)", Price = 32, Category = "1", Sold = 0 },

                new { Name = "Islatte", Price = 48, Category = "2", Sold = 0 },
                new { Name = "Ischai", Price = 46, Category = "2", Sold = 0 },
                new { Name = "Läsk (33 cl)", Price = 22, Category = "2", Sold = 0 },
                new { Name = "Mineralvatten", Price = 20, Category = "2", Sold = 0 },
                new { Name = "Smoothie (jordgubb & banan)", Price = 55, Category = "2", Sold = 0 },
                new { Name = "Färskpressad apelsinjuice", Price = 49, Category = "2", Sold = 0 },

                new { Name = "Kanelbulle", Price = 25, Category = "3", Sold = 0 },
                new { Name = "Chokladboll", Price = 18, Category = "3", Sold = 0 },
                new { Name = "Morotskaka (bit)", Price = 38, Category = "3", Sold = 0 },
                new { Name = "Cheesecake (bit)", Price = 42, Category = "3", Sold = 0 },
                new { Name = "Croissant", Price = 26, Category = "3", Sold = 0 },
                new { Name = "Muffins (blåbär)", Price = 28, Category = "3", Sold = 0 },

                new { Name = "Smörgås (ost & skinka)", Price = 38, Category = "4", Sold = 0 },
                new { Name = "Räksmörgås", Price = 69, Category = "4", Sold = 0 },
                new { Name = "Panini (kyckling & pesto)", Price = 58, Category = "4", Sold = 0 },
                new { Name = "Soppa med bröd", Price = 65, Category = "4", Sold = 0 },
                new { Name = "Quinoasallad", Price = 72, Category = "4", Sold = 0 },
            };

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var tx = connection.BeginTransaction())
                using (var cmd = new SQLiteCommand(@"INSERT INTO Products (Name, Price, Category, Sold) 
                                           VALUES (@name, @price, @category, @sold)", connection, tx))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@name"));
                    cmd.Parameters.Add(new SQLiteParameter("@price"));
                    cmd.Parameters.Add(new SQLiteParameter("@category"));
                    cmd.Parameters.Add(new SQLiteParameter("@sold"));

                    foreach (var product in products)
                    {
                        cmd.Parameters["@name"].Value = product.Name;
                        cmd.Parameters["@price"].Value = product.Price;
                        cmd.Parameters["@category"].Value = GetProductCategory(product.Category);
                        cmd.Parameters["@sold"].Value = product.Sold;
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
            }
        }

        public static string GetProductCategory(string productCategoryId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string selectCategoryQuery = $"SELECT CategoryName FROM ProductCategories WHERE Id = '{int.Parse(productCategoryId)}'";
                using (var cmd = new SQLiteCommand(selectCategoryQuery, connection))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    string category = "";
                    while (reader.Read())
                    {
                        category = reader.GetString(reader.GetOrdinal("CategoryName"));
                        System.Diagnostics.Debug.WriteLine("Log: " + category);
                    }
                    return category;
                }
            }
        }

        public static void AddSold(ObservableCollection<CartItem> allItems)
        {
            using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                foreach (var item in allItems)
                {
                    string query = $"UPDATE Products SET Sold = Sold + '{item.amount}' WHERE Name = '{item.name}'";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void AddReceipt(Receipt receipt)
        {
            using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();

                string insertReceiptQuery = @"
                INSERT INTO Receipts (Time, ArticleCount, ReceiptTotal, Subtotal, Saletax, PdfFormattedTime)
                VALUES (@time, @articleCount, @receiptTotal, @subtotal, @saletax, @pdfFormattedTime)";

                using (var cmd = new SQLiteCommand(insertReceiptQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@time", receipt.time);
                    cmd.Parameters.AddWithValue("@articleCount", receipt.articleCount);
                    cmd.Parameters.AddWithValue("@receiptTotal", receipt.receiptTotal);
                    cmd.Parameters.AddWithValue("@subtotal", receipt.subtotal);
                    cmd.Parameters.AddWithValue("@saletax", receipt.saleTax);
                    cmd.Parameters.AddWithValue("@pdfFormattedTime", receipt.PDFFormattedTime);

                    cmd.ExecuteNonQuery();

                    // Gets the generated receipt number
                    receipt.receiptNumber = (int)connection.LastInsertRowId;
                }
            }

            // Insert receipt products
            using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                foreach (var item in receipt.receiptProducts)
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

            using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
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
            using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        int intResult = Convert.ToInt32(result);
                        return intResult;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        public static string GetPdfFormattedTime(int receiptNumber)
        {
            string query = "SELECT PdfFormattedTime FROM Receipts WHERE ReceiptNumber = @receiptNumber";
            using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@receiptNumber", receiptNumber);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                    else
                    {
                        throw new Exception("Receipt not found");
                    }
                }
            }
        }
    }
}
