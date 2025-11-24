using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace TE4POS
{
    public partial class MainWindow : Window
    {
        // A list of all products available in the store (binds to the UI)
        public ObservableCollection<Product> AllProducts { get; set; }

        // A list of all products added to the cart (also binds to the UI)
        public ObservableCollection<CartItem> ShoppingCart { get; set; }

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

            // Creates a list with all products
            AllProducts = new ObservableCollection<Product>
            {
                new Product { Name = "Bryggkaffe (liten)", Price = 28, Category = "Varma drycker" },
                new Product { Name = "Bryggkaffe (stor)", Price = 34, Category = "Varma drycker" },
                new Product { Name = "Cappuccino", Price = 42, Category = "Varma drycker" },
                new Product { Name = "Latte", Price = 46, Category = "Varma drycker" },
                new Product { Name = "Varm choklad med grädde", Price = 45, Category = "Varma drycker" },
                new Product { Name = "Te (svart, grönt eller örtte)", Price = 32, Category = "Varma drycker" },
                new Product { Name = "Islatte", Price = 48, Category = "Kalla drycker" },
                new Product { Name = "Ischai", Price = 46, Category = "Kalla drycker" },
                new Product { Name = "Läsk (33 cl)", Price = 22, Category = "Kalla drycker" },
                new Product { Name = "Mineralvatten", Price = 20, Category = "Kalla drycker" },
                new Product { Name = "Smoothie (jordgubb & banan)", Price = 55, Category = "Kalla drycker" },
                new Product { Name = "Färskpressad apelsinjuice", Price = 49, Category = "Kalla drycker" },
                new Product { Name = "Kanelbulle", Price = 25, Category = "Bakverk" },
                new Product { Name = "Chokladboll", Price = 18, Category = "Bakverk" },
                new Product { Name = "Morotskaka (bit)", Price = 38, Category = "Bakverk" },
                new Product { Name = "Cheesecake (bit)", Price = 42, Category = "Bakverk" },
                new Product { Name = "Croissant", Price = 26, Category = "Bakverk" },
                new Product { Name = "Muffins (blåbär)", Price = 28, Category = "Bakverk" },
                new Product { Name = "Smörgås (ost & skinka)", Price = 38, Category = "Enkel mat" },
                new Product { Name = "Räksmörgås", Price = 69, Category = "Enkel mat" },
                new Product { Name = "Panini (kyckling & pesto)", Price = 58, Category = "Enkel mat" },
                new Product { Name = "Soppa med bröd", Price = 65, Category = "Enkel mat" },
                new Product { Name = "Quinoasallad", Price = 72, Category = "Enkel mat" },
            };

            // An empty cart
            ShoppingCart = new ObservableCollection<CartItem>{};

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
            int currentReceiptNumber = 0;

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

            // Adds receipt number and increments the total receipt number
            currentReceiptNumber = ((App)Application.Current).totalReceiptNumber++;
            currentReceipt.receiptNumber = currentReceiptNumber;

            // Adds the receipt to the receipt list
            ((App)Application.Current).AllReceipts.Add(currentReceipt);

            //Clears cart and cart price total for next order
            ShoppingCart.Clear();
            ShoppingCartTotal.Text = ShoppingCartTotalPrice.ToString();
        }
        private void ReceiptWindow_Click(object sender, RoutedEventArgs e)
        {
            // opens receipt window
            ReceiptWindow objReceiptWindow = new ReceiptWindow(((App)Application.Current).AllReceipts);
            this.Close();
            objReceiptWindow.Show();
        }
    }

    

    public class Product
    {
        public string Name { get; set; } = "";
        public int Price { get; set; }
        public string Category { get; set; } = "";

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
        public List<ReceiptProduct> ReceiptProducts { get; set; } = new List<ReceiptProduct>();
    }

    public class ReceiptProduct
    {
        public string receiptName { get; set; } = "";
        public int receiptPrice { get; set; }
        public int receiptAmount { get; set; }
        public int receiptProductTotal { get; set; }
    }
}