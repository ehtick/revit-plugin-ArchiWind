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
    public class CreateSimulation : ExternalCommand
    {
        public override void Execute()
        {
            var viewModel = new CreateSimulationViewModel(Document);
            var view = new CreateSimulationView(viewModel);
            view.ShowDialog();
        }
    }
}