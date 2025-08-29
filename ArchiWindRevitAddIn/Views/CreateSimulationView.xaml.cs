using ArchiWindRevitAddIn.ViewModels;
using System.Windows;

namespace ArchiWindRevitAddIn.Views
{
    public sealed partial class CreateSimulationView : Window
    {
        public CreateSimulationView(CreateSimulationViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();

            //viewModel.RequestClose += OnRequestClose;
        }

        //private void OnRequestClose(bool success)
        //{
        //    DialogResult = success;
        //    Close();
        //}

        //protected override void OnClosed(EventArgs e)
        //{
        //    // Clean up the event subscription to prevent memory leaks
        //    if (DataContext is CreateSimulationViewModel viewModel)
        //    {
        //        viewModel.RequestClose -= OnRequestClose;
        //    }
        //    base.OnClosed(e);
        //}
    }
}