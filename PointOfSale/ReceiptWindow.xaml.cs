using System.Collections.ObjectModel;
using System.Windows;


namespace TE4POS
{
    /// <summary>
    /// Interaction logic for ReceiptWindow.xaml
    /// </summary>
    public partial class ReceiptWindow : Window
    {
        public ObservableCollection<Receipt> ReceiptList { get; set; }
        public ReceiptWindow(ObservableCollection<Receipt> ReceiveReceipts)
        {
            
            InitializeComponent();

            ReceiptList = ReceiveReceipts;
            foreach (var item in ReceiptList)
            {
                System.Diagnostics.Debug.WriteLine(item.receiptName);
                System.Diagnostics.Debug.WriteLine(item.receiptPrice);
                System.Diagnostics.Debug.WriteLine(item.receiptAmount);
            }

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