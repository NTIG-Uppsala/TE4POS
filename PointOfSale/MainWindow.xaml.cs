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
                new Product { Id = 1, Name = "Bryggkaffe (liten)", Price = 28, Category = "Varma drycker" },
                new Product { Id = 2, Name = "Bryggkaffe (stor)", Price = 34, Category = "Varma drycker" },
                new Product { Id = 3, Name = "Cappuccino", Price = 42, Category = "Varma drycker" },
                new Product { Id = 4, Name = "Latte", Price = 46, Category = "Varma drycker" },
                new Product { Id = 5,  Name = "Varm choklad med grädde", Price = 45, Category = "Varma drycker" },
                new Product { Id = 6,  Name = "Te (svart, grönt eller örtte)", Price = 32, Category = "Varma drycker" },
                new Product { Id = 7, Name = "Islatte", Price = 48, Category = "Kalla drycker" },
                new Product { Id = 8, Name = "Ischai", Price = 46, Category = "Kalla drycker" },
                new Product { Id = 9, Name = "Läsk (33 cl)", Price = 22, Category = "Kalla drycker" },
                new Product { Id = 10, Name = "Mineralvatten", Price = 20, Category = "Kalla drycker" },
                new Product { Id = 11, Name = "Smoothie (jordgubb & banan)", Price = 55, Category = "Kalla drycker" },
                new Product { Id = 12, Name = "Färskpressad apelsinjuice", Price = 49, Category = "Kalla drycker" },
                new Product { Id = 13, Name = "Kanelbulle", Price = 25, Category = "Bakverk" },
                new Product { Id = 14, Name = "Chokladboll", Price = 18, Category = "Bakverk" },
                new Product { Id = 15, Name = "Morotskaka (bit)", Price = 38, Category = "Bakverk" },
                new Product { Id = 16, Name = "Cheesecake (bit)", Price = 42, Category = "Bakverk" },
                new Product { Id = 17, Name = "Croissant", Price = 26, Category = "Bakverk" },
                new Product { Id = 18, Name = "Muffins (blåbär)", Price = 28, Category = "Bakverk" },
                new Product { Id = 19, Name = "Smörgås (ost & skinka)", Price = 38, Category = "Enkel mat" },
                new Product { Id = 20, Name = "Räksmörgås", Price = 69, Category = "Enkel mat" },
                new Product { Id = 21, Name = "Panini (kyckling & pesto)", Price = 58, Category = "Enkel mat" },
                new Product { Id = 22, Name = "Soppa med bröd", Price = 65, Category = "Enkel mat" },
                new Product { Id = 23, Name = "Quinoasallad", Price = 72, Category = "Enkel mat" },
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
                    // If the product is already in the cart, increase the amount
                    existingItem.Amount++;
                    ShoppingCartTotal.Text += existingItem.Price;

                }
                else
                {
                    // If the product is NOT in the cart, add it with amount 1
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
            foreach (var cartItem in ShoppingCart)
            {
                // Reset the amount of each product in the cart to 1
                cartItem.Amount = 1;
            }

            ShoppingCart.Clear();
            ShoppingCartTotal.Text = ShoppingCartTotalPrice.ToString();
        }

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            foreach (var cartItem in ShoppingCart)
            {
                // Reset the amount of each product in the cart to 1
                cartItem.Amount = 1;
            }

            ShoppingCart.Clear();
            ShoppingCartTotal.Text = ShoppingCartTotalPrice.ToString();
        }

        private void WrapPanel_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Price { get; set; }
        public string Category { get; set; } = "";

    }
    public class CartItem : Product, INotifyPropertyChanged
    {
        // Backing field for Amount (needed for OnPropertyChanged)
        private int _amount;

        // Amount property notifies the UI when the value changes
        public int Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged();
            }
        }
        // Standard INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Notify WPF that a property changed so the UI updates
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}