using ArchiWindRevitAddIn.ViewModels;
using ArchiWindRevitAddIn.Views;
using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;

namespace ArchiWindRevitAddIn.Commands
{
    [Transaction(TransactionMode.ReadOnly)]
    public class AccountSettings : ExternalCommand
    {
        public override void Execute()
        {
            var viewModel = new AccountSettingsViewModel();
            var view = new AccountSettingsView(viewModel);
            view.ShowDialog();
        }
    }
}
