using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;


namespace TE4POS
{
    /// <summary>
    /// Interaction logic for ReceiptWindow.xaml
    /// </summary>
    public partial class ReceiptWindow : Window
    {
        public ObservableCollection<Receipt> ReceiptList { get; set; }

        public ReceiptWindow(ObservableCollection<Receipt> receiveReceipts)
        {
            InitializeComponent();

            ReceiptList = receiveReceipts;

            DataContext = this;
        }

        private void ToMainWindow(object sender, RoutedEventArgs e)
        {
            
            MainWindow objMainWindow = new MainWindow();
            this.Close();
            objMainWindow.Show();
        }
    }
}