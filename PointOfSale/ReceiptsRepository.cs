using System.Collections.ObjectModel;
using System.Data.SQLite;
using static TE4POS.MainWindow;
using TE4POS;

namespace ReceiptsRepository
{
    public interface IReceiptsRepository
    {
        IEnumerable<Receipt> GetAllReceipts();
    }

    public class SQLiteReceiptsRepository : IReceiptsRepository
    {
        public ObservableCollection<Receipt> AllReceipts { get; private set; }
            = new ObservableCollection<Receipt>();

        private static readonly string filePath = "Databases/Database.db";
        private static readonly string connectionString = @"Data Source=" + filePath + ";Version=3";

        private static readonly string testFilePath = "Databases/TestDatabase.db";
        private static readonly string testConnectionString = @"Data Source=" + testFilePath + ";Version=3";

        private static string currentConnectionString = "";

        public IEnumerable<Receipt> GetAllReceipts()
        {
            if (App.isTest)
            {
                currentConnectionString = testConnectionString;
            }
            else
            {
                currentConnectionString = connectionString;
            }
            AllReceipts.Clear(); // important might cause duplication otherwise

            using (var connection = new SQLiteConnection(currentConnectionString))
            {
                connection.Open();
                string receiptsQuery = "SELECT * FROM Receipts";

                using (SQLiteCommand command = new SQLiteCommand(receiptsQuery, connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AllReceipts.Add(new Receipt()
                        {
                            receiptNumber = reader.GetInt32(reader.GetOrdinal("ReceiptNumber")),
                            articleCount = reader.GetInt32(reader.GetOrdinal("ArticleCount")),
                            receiptTotal = reader.GetInt32(reader.GetOrdinal("ReceiptTotal")),
                            subtotal = reader.GetFloat(reader.GetOrdinal("Subtotal")),
                            saleTax = reader.GetFloat(reader.GetOrdinal("SaleTax")),
                            PDFFormattedTime = reader.GetString(reader.GetOrdinal("PdfFormattedTime")),
                            time = reader.GetString(reader.GetOrdinal("Time"))
                        });
                    }
                }
                string receiptProductsQuery = "SELECT * FROM ReceiptProducts";
                using (SQLiteCommand command = new SQLiteCommand(receiptProductsQuery, connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int Id = reader.GetInt32(reader.GetOrdinal("ProductId"));
                        string productName = GetProductName(Id, connection);
                        int receiptNumber = reader.GetInt32(reader.GetOrdinal("ReceiptNumber"));

                        Receipt? receipt = AllReceipts.FirstOrDefault(r => r.receiptNumber == receiptNumber);

                        if (receipt != null)
                        {
                            // Always create list if null
                            if (receipt.receiptProducts == null)
                                receipt.receiptProducts = new List<ReceiptProduct>();

                            
                            receipt.receiptProducts.Add(new ReceiptProduct()
                            {
                                receiptName = productName,
                                receiptAmount = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                receiptPrice = reader.GetInt32(reader.GetOrdinal("UnitPrice")),
                                receiptProductTotal = reader.GetInt32(reader.GetOrdinal("TotalPrice")),
                            });
                        }
                    }
                }
            }
            return AllReceipts;
        }
        public static string GetProductName(int id, SQLiteConnection connection)
        {
            string query = "SELECT Name FROM Products WHERE Id = @ProductId";

            using (SQLiteCommand cmd = new SQLiteCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@ProductId", id);

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    return result.ToString()!;
                }
                else
                {
                    throw new Exception("Product not found");
                }
            }
        }
    }

}
