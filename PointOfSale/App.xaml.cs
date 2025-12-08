using QuestPDF;
using QuestPDF.Infrastructure;
using System.Collections.ObjectModel;
using System.Windows;

namespace TE4POS
{
    public partial class App : Application
    {
        public static bool isTest = true;
        protected override void OnStartup(StartupEventArgs e)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            base.OnStartup(e);
        }
    }
}
