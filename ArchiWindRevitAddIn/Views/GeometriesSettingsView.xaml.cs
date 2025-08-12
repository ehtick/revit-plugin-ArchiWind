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
    }
}
