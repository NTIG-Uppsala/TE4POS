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
        private Products products;
        public MainWindow()
        {
            InitializeComponent();
            products = new Products();
            DataContext = products;
        }

        private void addAmount(object sender, RoutedEventArgs e)
        {

        }
    }
}
