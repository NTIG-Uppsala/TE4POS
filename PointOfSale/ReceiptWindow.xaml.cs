using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TE4POS;

namespace TE4POS
{
    /// <summary>
    /// Interaction logic for ReceiptWindow.xaml
    /// </summary>
    public partial class ReceiptWindow : Window
    {
        public ObservableCollection<Receipt> ReceiptList;
        public ReceiptWindow(ObservableCollection<Receipt> ReceiveReceipts)
        {
            ReceiptList = ReceiveReceipts;
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine(ReceiptList);
        }

        private void MainWindow(object sender, RoutedEventArgs e)
        {
            
            MainWindow objMainWindow = new MainWindow();
            this.Close();
            objMainWindow.Show();
        }
    }
}
