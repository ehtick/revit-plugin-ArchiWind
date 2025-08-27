using ArchiWindRevitAddIn.Views;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using Nice3point.Revit.Toolkit.External;

namespace ArchiWindRevitAddIn.Commands
{
    [Transaction(TransactionMode.ReadOnly)]
    public class ExportStls : ExternalCommand
    {
        public override void Execute()
        {
            var buildingView = Utils.FindView(Document, Utils.BUILDING_VIEW);
            var surroundingsView = Utils.FindView(Document, Utils.SURROUNDINGS_VIEW);
            var terrainView = Utils.FindView(Document, Utils.TERRAIN_VIEW);
            var vegetationView = Utils.FindView(Document, Utils.VEGETATION_VIEW);

            if (buildingView is null || surroundingsView is null || terrainView is null || vegetationView is null)
            {
                TaskDialog.Show("Error",
                                $"One or more 3D views are missing.\nClick on the \"Preview\" button of the add-in to create them.",
                                TaskDialogCommonButtons.Ok,
                                TaskDialogResult.Ok);
                return;
            }

#if REVIT2025_OR_GREATER
            var dialog = new OpenFolderDialog()
            {
                ValidateNames = true,
                Multiselect = false,
            };

            if (dialog.ShowDialog() is false)
            {
                TaskDialog.Show("Error",
                                $"You must select a folder to continue.",
                                TaskDialogCommonButtons.Ok,
                                TaskDialogResult.Ok);
                return;
            }

            string dir = dialog.FolderName;
#else
            using var dialog = new FolderBrowserDialog()
            {
            };

            if (dialog.ShowDialog() is not DialogResult.OK)
            {
                TaskDialog.Show("Error",
                                $"You must select a folder to continue.",
                                TaskDialogCommonButtons.Ok,
                                TaskDialogResult.Ok);
                return;
            }

            string dir = dialog.SelectedPath;
#endif

            try
            {
                Utils.ExportViewAsStl(Document, buildingView.Id, dir, "building.stl");
                Utils.ExportViewAsStl(Document, surroundingsView.Id, dir, "surroundings.stl");
                Utils.ExportViewAsStl(Document, terrainView.Id, dir, "terrain.stl");
                Utils.ExportViewAsStl(Document, vegetationView.Id, dir, "vegetation.stl");
            }
            catch (Exception ex)
            {
                Autodesk.Revit.UI.TaskDialog.Show("Error",
                                $"An error occured, please report to the developer: \n\n{ex.GetType()}\n{ex.Message}",
                                TaskDialogCommonButtons.Ok,
                                TaskDialogResult.Ok);
                return;
            }


            Autodesk.Revit.UI.TaskDialog.Show("Done",
                                $"STLs exported to {dir}.",
                                TaskDialogCommonButtons.Ok,
                                TaskDialogResult.Ok);
        }
    }
}
