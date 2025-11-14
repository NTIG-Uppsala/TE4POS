using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace TE4POS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Product> AllProducts { get; set; }
        public ObservableCollection<Product> ProductsInCart { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            AllProducts = new ObservableCollection<Product>
            {
                new Product { Name = "Apple", Price = 10, Amount = 1 },
                new Product { Name = "Banana", Price = 5, Amount = 2 },
                new Product { Name = "Orange", Price = 8, Amount = 1 }
            };

            ProductsInCart = new ObservableCollection<Product>{};

            DataContext = this;
        }

        private void AddAmount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is Product product)
            {
                new ProductInCart 
                {
                    Name = product.Name,
                    Price = product.Price,
                    Amount = product.Amount
                };
                if (!ProductsInCart.Contains(product))
                    ProductsInCart.Add(product);
                else
                {
                    product.Amount++;
                }
            }
        }
    }

    public class Product : INotifyPropertyChanged
    {
         public int _amount;
         public string Name { get; set; }
         public int Price { get; set; }

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
    public class ProductInCart : Product
    {

    }
}