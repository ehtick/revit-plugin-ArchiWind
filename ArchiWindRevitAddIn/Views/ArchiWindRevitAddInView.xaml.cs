using ArchiWindRevitAddIn.ViewModels;

namespace ArchiWindRevitAddIn.Views
{
    public sealed partial class ArchiWindRevitAddInView
    {
        public ArchiWindRevitAddInView(ArchiWindRevitAddInViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}