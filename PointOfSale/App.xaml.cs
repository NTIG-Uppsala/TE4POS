using QuestPDF;
using QuestPDF.Infrastructure;
using System.Collections.ObjectModel;
using System.Windows;

namespace TE4POS
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            base.OnStartup(e);
        }

        public System.Collections.ObjectModel.ObservableCollection<Receipt> AllReceipts { get; set; } = new System.Collections.ObjectModel.ObservableCollection<Receipt>();
        public int totalReceiptNumber = 1;
    }
}
