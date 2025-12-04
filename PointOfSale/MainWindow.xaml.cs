using QuestPDF.Companion;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ProductsRepository;
using ReceiptsRepository;
using static TE4POS.MainWindow;
using System.Diagnostics;
using Microsoft.Win32;

namespace TE4POS
{
    public partial class MainWindow : Window
    {
        private SQLiteProductsRepository productsRepo = new SQLiteProductsRepository();
        private SQLiteReceiptsRepository receiptsRepo = new SQLiteReceiptsRepository();

        // A list of all products available in the store
        public ObservableCollection<Product> AllProducts { get; set; }

        // A list of all receipts 
        public ObservableCollection<Receipt> ReceiptList { get; set; }

        // A list of all products added to the cart
        public ObservableCollection<CartItem> ShoppingCart { get; set; }

        public double VAT = 1.12;

        public int shoppingCartTotalPrice
        {
            get
            {
                return ShoppingCart.Sum(item => item.price * item.amount);
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // Creates the database file if it doesn't exist
            DatabaseHelper.InitializeDatabase();

            // Loads data from the database
            productsRepo.GetAllProducts();
            receiptsRepo.GetAllReceipts();

            // bind to the ObservableCollection
            AllProducts = productsRepo.AllProducts; 
            ReceiptList = receiptsRepo.AllReceipts; 

            // An empty cart
            ShoppingCart = new ObservableCollection<CartItem> { };

            // Makes bindings look for properties inside this class
            DataContext = this;
        }

        private void AddAmountClick(object sender, RoutedEventArgs e)
        {
            // Check that the sender is a UI element and that it has a Product as DataContext
            if (sender is FrameworkElement fe && fe.DataContext is Product product)
            {
                var existingItem = ShoppingCart.FirstOrDefault(x => x.name == product.name);

                if (existingItem != null)
                {
                    existingItem.amount++;
                }
                else
                {
                    ShoppingCart.Add(new CartItem
                    {
                        name = product.name,
                        price = product.price,
                        amount = 1
                    });
                }
                // Update the total price (displayed)
                ShoppingCartTotal.Text = shoppingCartTotalPrice.ToString();
            }
        }

        private void ResetCartClick(object sender, RoutedEventArgs e)
        {   
            ShoppingCart.Clear();
            ShoppingCartTotal.Text = shoppingCartTotalPrice.ToString();
        }

        private async void CheckoutClick(object sender, RoutedEventArgs e)
        {
            if (ShoppingCart.Count != 0)
            {
                var time = DateTime.Now;

                // Had to be = 0 otherwise things wouldn't work for some reason
                int receiptArticleCount = 0;
                int receiptTotalCost = 0;

                // Makes a new receipt object
                var currentReceipt = new Receipt { };

                // Adds each item in the cart to the receipt
                foreach (CartItem item in ShoppingCart)
                {
                    // Finds name, price, and how many of the item is int the cart
                    string receiptProductName = item.name;
                    int receiptProductPrice = item.price;
                    int receiptProductAmount = item.amount;

                    // Adds a total price based on item price and amount
                    int totalProductAmountPrice = receiptProductPrice * receiptProductAmount;

                    // Puts all of the cart items info into one object
                    var receiptProduct = new ReceiptProduct
                    {
                        receiptName = receiptProductName,
                        receiptPrice = receiptProductPrice,
                        receiptAmount = receiptProductAmount,
                        receiptProductTotal = totalProductAmountPrice,
                    };
                    // Adds the item object to the receipt
                    currentReceipt.receiptProducts.Add(receiptProduct);

                    // Adds the number of articles to the total number of articles in the receipt
                    receiptArticleCount += receiptProductAmount;
                    // Adds the price to the total receipt price
                    receiptTotalCost += totalProductAmountPrice;
                }

                // Adds the current time, article count, and total price to the receipt
                currentReceipt.time = time.ToString("yyyy-MM-dd HH:mm:ss");
                currentReceipt.PDFFormattedTime = time.ToString("yyyyMMdd_HHmmss_");
                currentReceipt.articleCount = receiptArticleCount;
                currentReceipt.receiptTotal = receiptTotalCost;

                // VAT Calculations 
                double beforeVAT = Math.Round(receiptTotalCost / VAT, 2);
                currentReceipt.subtotal = beforeVAT;
                currentReceipt.saleTax = Math.Round(receiptTotalCost - beforeVAT, 2);
            
                DatabaseHelper.AddReceipt(currentReceipt);

                // Adds the receipt to the receipt list
                ReceiptList.Add(currentReceipt);

                // Updates the stock in the database
                DatabaseHelper.RemoveStock(ShoppingCart);

                // Clears cart and cart price total for next order
                ShoppingCart.Clear();
                ShoppingCartTotal.Text = shoppingCartTotalPrice.ToString();
            }
        }

        WebBrowser browser = new WebBrowser();

        private void ReceiptClick(object sender, RoutedEventArgs e)
        {
            ShowReceipt.Children.Clear();
            int thisReceipt = int.Parse(((Button)sender).Tag.ToString());

            foreach (Receipt receipt in ReceiptList)
            {
                if (receipt.receiptNumber == thisReceipt)
                {
                    GenerateReceiptPDF(receipt.PDFFormattedTime, receipt.receiptNumber);

                    string thisReceiptTime = receipt.PDFFormattedTime;
                    string filename = $"{thisReceiptTime}_{thisReceipt}.pdf";

                    string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
                    string directory = Path.Combine(projectRoot, "Pdfs");
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    string filePath = Path.Combine(directory, filename);

                    ShowReceipt.Children.Add(browser);
                    browser.Navigate(filePath);
                    ShowReceipt.Opacity = 200;
                }
            }
        }

        public void GenerateReceiptPDF(string dateAndTime , int receiptNumber)
        {
            var receipt = ReceiptList[receiptNumber-1];

            var pdf = new ReceiptPdf(receipt);
            byte[] pdfBytes = pdf.GeneratePdf();

            string filename = $"{dateAndTime}_{receiptNumber}.pdf";
            string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
            string directory = Path.Combine(projectRoot, "Pdfs");
            string filePath = Path.Combine(directory, filename);

            Directory.CreateDirectory(directory);
            File.WriteAllBytesAsync(filePath, pdfBytes);
        }

        public void OpenReceiptFolderClick(object sender, RoutedEventArgs e)
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
            string directory = Path.Combine(projectRoot, "Pdfs");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Open folder in Explorer
            Process.Start(new ProcessStartInfo
            {
                FileName = directory,
                UseShellExecute = true,
                Verb = "open"
            });
        }



    public class Product
    {
        public int id { get; set; }
        public string name { get; set; } = "";
        public string category { get; set; } = "";
        public int stock { get; set; }
        public int price { get; set; }

            public string priceFormatted
            {
                get
                {
                    return string.Format("{0:F}", price);
                }
            }

            // Parameterless constructor for derived classes
            public Product()
            {

            }
        
            public Product(string productName, string productCategory, int productPrice)
            {
                name = productName;
                category = productCategory;
                price = productPrice;
            }
        }
    
        public class CartItem : Product, INotifyPropertyChanged
        {
            private int _amount;

            public int amount
            {
                get => _amount;
                set
                {
                    _amount = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class Receipt
        {
            public string time { get; set; } = "";
            public string PDFFormattedTime { get; set; } = "";
            public int receiptNumber { get; set; }
            public int articleCount { get; set; }
            public int receiptTotal { get; set; }
            public string receiptTotalFormatted
            {
                get
                {
                    return String.Format("{0:F}", receiptTotal);
                }
            }
            public double subtotal { get; set; }
            public string subtotalFormatted
            {
                get
                {
                    return String.Format("{0:F}", subtotal);
                }
            }
            public double saleTax { get; set; }
            public string saleTaxFormatted
            {
                get
                {
                    return String.Format("{0:F}", saleTax);
                }
            }
            public List<ReceiptProduct> receiptProducts { get; set; } = new List<ReceiptProduct>();
        }

        public class ReceiptProduct
        {
            public string receiptName { get; set; } = "";
            public int receiptAmount { get; set; }
            public int receiptPrice { get; set; }
            public string receiptPriceFormatted
            {
                get
                {
                    return String.Format("{0:F}", receiptPrice);
                }
            }
            public int receiptProductTotal { get; set; }
            public string receiptProductTotalFormatted
            {
                get
                {
                    return String.Format("{0:F}", receiptProductTotal);
                }
            }
        }
    }

    public class ReceiptPdf : IDocument
    {
        public Receipt Receipt { get; }

        public ReceiptPdf(Receipt receipt)
        {
            Receipt = receipt;
        }

        public DocumentMetadata GetMetadata() => new();
        public DocumentSettings GetSettings() => new();

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.ContinuousSize(400);

                page.Header()
                    .AlignCenter().Text($"Café Bullen").FontSize(24).Bold();

                page.Content().Column(col =>
                {

                    col.Item().AlignCenter().Text($"Torggatan 11");
                    col.Item().AlignCenter().Text($"123 34 Storstad");
                    col.Item().AlignCenter().Text($"Tel: 555 - 42 67 11");
                    col.Item().AlignCenter().Text($"Org. Nr: 769966-1899");

                    col.Item().PaddingTop(5).PaddingBottom(5).LineHorizontal(1).LineColor(QuestPDF.Helpers.Colors.Black).LineDashPattern(new float[] { 4f, 4f });

                    col.Item().AlignCenter().Text($"Tid: {Receipt.time}");
                    col.Item().AlignCenter().Text($"Kvittonr: {Receipt.receiptNumber}");
                    col.Item().AlignCenter().Text($"Kassa: 1");

                    col.Item().PaddingTop(5).PaddingBottom(5).LineHorizontal(1).LineColor(QuestPDF.Helpers.Colors.Black).LineDashPattern(new float[] { 4f, 4f });

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(5); // Product name
                            cols.RelativeColumn(1); // Amount
                            cols.RelativeColumn(1); // Price
                            cols.RelativeColumn(2); // Total
                        });

                        foreach (var item in Receipt.receiptProducts)
                        {
                            table.Cell().Text(item.receiptName);
                            table.Cell().AlignRight().Text($"{item.receiptAmount.ToString()}x");
                            table.Cell().Text($" {item.receiptPrice} kr");
                            table.Cell().AlignRight().Text($"{item.receiptProductTotal} kr");
                        }
                    });
                    col.Item().PaddingTop(5).LineHorizontal(1).LineColor(QuestPDF.Helpers.Colors.Black).LineDashPattern(new float[] { 4f, 4f });
                    col.Item().PaddingTop(5).PaddingBottom(5).LineHorizontal(1).LineColor(QuestPDF.Helpers.Colors.Black).LineDashPattern(new float[] { 4f, 4f });

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Antal art:");
                        row.AutoItem().AlignRight().Text($"{Receipt.articleCount}");
                    });

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"TOT:");
                        row.AutoItem().AlignRight().Text($"{Receipt.receiptTotalFormatted} kr");
                    });

                    col.Item().Text($"KORT");

                    col.Item().PaddingTop(5).PaddingBottom(5).LineHorizontal(1).LineColor(QuestPDF.Helpers.Colors.Black).LineDashPattern(new float[] { 4f, 4f });

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Moms");
                        row.RelativeItem().Text($"Belopp");
                        row.RelativeItem().Text($"Netto");
                        row.RelativeItem().Text($"Brutto");
                    });

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"12%");
                        row.RelativeItem().Text($"{Receipt.subtotalFormatted}");
                        row.RelativeItem().Text($"{Receipt.saleTaxFormatted}");
                        row.RelativeItem().Text($"{Receipt.receiptTotalFormatted}");
                    });
                });
            });
        }
    }
}
