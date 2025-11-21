using System.Collections.ObjectModel;
using System.Windows;

namespace TE4POS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public ObservableCollection<Receipt> AllReceipts { get; set; } = new ObservableCollection<Receipt>();
    }

}
