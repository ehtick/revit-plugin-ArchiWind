using ArchiWindRevitAddIn.ViewModels;
using System.Windows;

namespace ArchiWindRevitAddIn.Views
{
    public partial class AccountSettingsView : Window
    {
        public AccountSettingsView(AccountSettingsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
