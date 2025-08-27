using ArchiWindRevitAddIn.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace ArchiWindRevitAddIn.Views
{
    public partial class AccountSettingsView : Window
    {
        public AccountSettingsView(AccountSettingsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void OpenLink(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = e.Uri.ToString(),
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to open link: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                e.Handled = true;
            }
        }
    }
}
