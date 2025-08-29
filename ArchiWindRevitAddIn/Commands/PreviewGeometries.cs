using ArchiWindRevitAddIn.Views;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;

namespace ArchiWindRevitAddIn.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class PreviewGeometries : ExternalCommand
    {
        private const string TRANSACTION_NAME = "ArchiWind 3D views setup";

        public override void Execute()
        {
            Document doc = ActiveView.Document;

            if (ActiveView.ViewType != ViewType.ThreeD)
            {
                TaskDialog.Show("Error",
                                $"Please select the document's 3D view.",
                                TaskDialogCommonButtons.Ok,
                                TaskDialogResult.Ok);

                Result = Result.Failed;

                return;
            }

            using var t = new Transaction(doc, TRANSACTION_NAME);

            try
            {
                t.Start();

                var buildingView = Utils.CreateView(doc, ActiveView, Utils.BUILDING_VIEW);
                var surroundingsView = Utils.CreateView(doc, ActiveView, Utils.SURROUNDINGS_VIEW);
                var vegetationView = Utils.CreateView(doc, ActiveView, Utils.VEGETATION_VIEW);
                var terrainView = Utils.CreateView(doc, ActiveView, Utils.TERRAIN_VIEW);

                Utils.OnlyShowCategories(doc, buildingView, Models.Categories.DefaultBuildingCategories);
                Utils.OnlyShowCategories(doc, surroundingsView, Models.Categories.DefaultSurroundingsCategories);
                Utils.OnlyShowCategories(doc, vegetationView, Models.Categories.DefaultVegetationCategories);
                Utils.OnlyShowCategories(doc, terrainView, Models.Categories.DefaultTerrainCategories);

                t.Commit();

                UiDocument.RequestViewChange(buildingView);
                UiDocument.RequestViewChange(surroundingsView);
                UiDocument.RequestViewChange(vegetationView);
                UiDocument.RequestViewChange(terrainView);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error",
                                $"An error occured while building previews, please report to the developer: \n\n{ex.GetType()}\n{ex.Message}",
                                TaskDialogCommonButtons.Ok,
                                TaskDialogResult.Ok);

                if (t.GetStatus() == TransactionStatus.Started)
                {
                    t.RollBack();
                }

                Result = Result.Failed;

                return;
            }
        }
    }
}
