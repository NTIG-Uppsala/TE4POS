using System.Collections.ObjectModel;
using System.Windows;

namespace TE4POS
{
    public partial class App : Application {
        public ObservableCollection<Receipt> AllReceipts { get; set; } = new ObservableCollection<Receipt>();
        public int totalReceiptNumber = 1;
    }

}
