using ArchiWindRevitAddIn.ViewModels;
using ArchiWindRevitAddIn.Views;
using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;

namespace ArchiWindRevitAddIn.Commands
{
    /// <summary>
    ///     External command entry point
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    public class GeometriesSettings : ExternalCommand
    {
        public override void Execute()
        {
            var viewModel = new GeometriesSettingsViewModel();
            var view = new GeometriesSettingsView(viewModel);
            view.ShowDialog();
        }
    }
}