using ArchiWindRevitAddIn.ViewModels;
using ArchiWindRevitAddIn.Views;
using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;

namespace ArchiWindRevitAddIn.Commands
{
    /// <summary>
    ///     External command entry point
    /// </summary>
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class StartupCommand : ExternalCommand
    {
        public override void Execute()
        {
            var viewModel = new ArchiWindRevitAddInViewModel();
            var view = new ArchiWindRevitAddInView(viewModel);
            view.ShowDialog();
        }
    }
}