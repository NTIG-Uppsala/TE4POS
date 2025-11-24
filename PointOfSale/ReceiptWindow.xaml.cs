using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;


namespace TE4POS
{
    public partial class ReceiptWindow : Window
    {
        public ObservableCollection<Receipt> ReceiptList { get; set; }

        public ReceiptWindow(ObservableCollection<Receipt> receiveReceipts)
        {
            InitializeComponent();

            ReceiptList = ((App)Application.Current).AllReceipts;

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