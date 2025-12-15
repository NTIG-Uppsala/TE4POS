using QuestPDF;
using QuestPDF.Infrastructure;
using System.Windows;

namespace TE4POS
{
    public partial class App : Application
    {
        public static bool isTest = false; // Must rebuild solution after setting true/false
        protected override void OnStartup(StartupEventArgs e)
        {
            Settings.License = LicenseType.Community;

            base.OnStartup(e);
        }
    }
}
