using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TE4POS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int kaffe = 0;
        int total;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Add_Coffee_Click(object sender, RoutedEventArgs e)
        {
            kaffe++;
            kaffenr.Clear();
            kaffenr.AppendText(kaffe.ToString());
            sum.Clear();
            total = kaffe * 49;
            sum.AppendText(total.ToString());
        }

        private void Remove_Coffee_Click(object sender, RoutedEventArgs e)
        {
            if (kaffe > 0)
            {
                kaffe--;
                kaffenr.Clear();
                kaffenr.AppendText(kaffe.ToString());
            }
            sum.Clear();
            total = kaffe * 49;
            sum.AppendText(total.ToString());
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            kaffe = 0;
            kaffenr.Clear();
            kaffenr.AppendText(kaffe.ToString());
            sum.Clear();
        }
    }
}
