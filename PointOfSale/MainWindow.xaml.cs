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

        public int ShoppingCartTotalPrice
        {
            get
            {
                return ShoppingCart.Sum(item => item.Price * item.Amount);
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

        private void AddAmount_Click(object sender, RoutedEventArgs e)
        {
            // Check that the sender is a UI element and that it has a Product as DataContext
            if (sender is FrameworkElement fe && fe.DataContext is Product product)
            {

                var existingItem = ShoppingCart.FirstOrDefault(x => x.Name == product.Name);

                if (existingItem != null)
                {
                    existingItem.Amount++;

                }
                else
                {
                    ShoppingCart.Add(new CartItem
                    {
                        Name = product.Name,
                        Price = product.Price,
                        Amount = 1
                    });
                    
                }
                // Update the total price (displayed)
                ShoppingCartTotal.Text = ShoppingCartTotalPrice.ToString();
            }
        }

        private void ResetCart_Click(object sender, RoutedEventArgs e)
        {   
            ShoppingCart.Clear();
            ShoppingCartTotal.Text = ShoppingCartTotalPrice.ToString();
        }

        private async void Checkout_Click(object sender, RoutedEventArgs e)
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
                    string receiptProductName = item.Name;
                    int receiptProductPrice = item.Price;
                    int receiptProductAmount = item.Amount;

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
                    currentReceipt.ReceiptProducts.Add(receiptProduct);

                    // Adds the number of articles to the total number of articles in the receipt
                    receiptArticleCount += receiptProductAmount;
                    // Adds the price to the total receipt price
                    receiptTotalCost += totalProductAmountPrice;
                }

                // Adds the current time, article count, and total price to the receipt
                currentReceipt.Time = time.ToString("yyyy-MM-dd HH:mm:ss");
                currentReceipt.PDFFormatedTime = time.ToString("yyyyMMdd_HHmmss_");
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
                ShoppingCartTotal.Text = ShoppingCartTotalPrice.ToString();

                int receiptNumber = DatabaseHelper.GetCurrentReceiptNumber();
                generateReceiptPDF(currentReceipt.PDFFormatedTime, receiptNumber);
            }
        }

        System.Windows.Controls.WebBrowser browser = new System.Windows.Controls.WebBrowser();

        private void Receipt_Click(object sender, RoutedEventArgs e)
        {
            showReceipt.Children.Clear();
            int thisReceipt = int.Parse(((Button)sender).Tag.ToString());

            foreach(Receipt receipt in ReceiptList)
            {
                
                if (receipt.receiptNumber == thisReceipt)
                {
                    string thisReceiptTime = receipt.PDFFormatedTime;
                    string filename = $"{thisReceiptTime}_{thisReceipt}.pdf";

                    string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
                    string directory = Path.Combine(projectRoot, "Pdfs");
                    string filePath = Path.Combine(directory, filename);

                    showReceipt.Children.Add(browser);
                    browser.Navigate(filePath);
                    showReceipt.Opacity = 200;
                }
            }
        }

        public async void generateReceiptPDF(string dateAndTime , int receiptNumber)
        {
            var receipt = ReceiptList[receiptNumber-1];

            var pdf = new ReceiptPdf(receipt);
            byte[] pdfBytes = await System.Threading.Tasks.Task.Run(() => pdf.GeneratePdf());

            string safetime = dateAndTime;
            string filename = $"{safetime}_{receiptNumber}.pdf";
            string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
            string directory = Path.Combine(projectRoot, "Pdfs");
            string filePath = Path.Combine(directory, filename);

            Directory.CreateDirectory(directory);
            
            await File.WriteAllBytesAsync(filePath, pdfBytes);
        }



    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public int Stock { get; set; }
        public int Price { get; set; }

        public string PriceFormatted
        {
            get
            {
                return string.Format("{0:F}", Price);
            }
        }

        // Parameterless constructor for derived classes (CartItem for now)
        public Product()
        {

        }
        
        public Product(string name, string category, int price)
        {
            Name = name;
            Category = category;
            Price = price;
        }
    }
    public class CartItem : Product, INotifyPropertyChanged
    {
        private int _amount;

        public int Amount
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
            public string Time { get; set; } = "";
            public string PDFFormatedTime { get; set; } = "";
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
            public List<ReceiptProduct> ReceiptProducts { get; set; } = new List<ReceiptProduct>();
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

                    col.Item().AlignCenter().Text($"Tid: {Receipt.Time}");
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

                        foreach (var item in Receipt.ReceiptProducts)
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
