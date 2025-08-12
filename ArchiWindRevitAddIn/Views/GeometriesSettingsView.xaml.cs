using ArchiWindRevitAddIn.ViewModels;
using System.Windows;

namespace ArchiWindRevitAddIn.Views
{
    public partial class GeometriesSettingsView : Window
    {
        public GeometriesSettingsView(GeometriesSettingsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void BTN_SelectAll_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BTN_DeselectAll_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BTN_Default_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BTN_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BTN_Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
