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

            // Create some example products
            AllProducts = new ObservableCollection<Product>
            {
                new Product { Name = "Apple", Price = 10 },
                new Product { Name = "Banana", Price = 5 },
                new Product { Name = "Orange", Price = 8 }
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
    }

    public class Product
    {
        public string Name { get; set; }
        public int Price { get; set; }

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