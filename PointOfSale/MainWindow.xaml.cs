using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ProductsRepository;
using ReceiptsRepository;

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
            ShoppingCart = new ObservableCollection<CartItem>{ };

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

        private void Checkout_Click(object sender, RoutedEventArgs e)
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
        }

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