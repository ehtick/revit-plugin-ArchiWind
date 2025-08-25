using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;
using System.Collections.Immutable;

namespace ArchiWindRevitAddIn.Commands
{
    /// <summary>
    ///     External command entry point
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PreviewGeometries : ExternalCommand
    {
        private const string TRANSACTION_NAME = "ArchiWind 3D views setup";

        private const string BUILDING_VIEW = "ArchiWind - Building View";
        private const string SURROUNDINGS_VIEW = "ArchiWind - Surroundings View";
        private const string TERRAIN_VIEW = "ArchiWind - Terrain View";
        private const string VEGETATION_VIEW = "ArchiWind - Vegetation View";

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

                var buildingView = CreateView(BUILDING_VIEW, doc, ActiveView);
                var surroundingsView = CreateView(SURROUNDINGS_VIEW, doc, ActiveView);
                var vegetationView = CreateView(VEGETATION_VIEW, doc, ActiveView);
                var terrainView = CreateView(TERRAIN_VIEW, doc, ActiveView);

                OnlyShowCategories(doc, buildingView, Models.Categories.DefaultBuildingCategories);
                OnlyShowCategories(doc, surroundingsView, Models.Categories.DefaultSurroundingsCategories);
                OnlyShowCategories(doc, vegetationView, Models.Categories.DefaultVegetationCategories);
                OnlyShowCategories(doc, terrainView, Models.Categories.DefaultTerrainCategories);

                t.Commit();
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

            //ProjectLocation loc = currentDocument.ActiveProjectLocation;

            //var terrainView = this.ActiveView.Duplicate(ViewDuplicateOption.Duplicate);
            //new FilteredElementCollector(currentDocument, terrainView).

            //STLExportOptions exportOptions = new STLExportOptions();
            //exportOptions.TargetUnit = ExportUnit.Meter;
            //exportOptions.SetTessellationSettings(ExportResolution.Coarse);
            //exportOptions.ExportBinary = true;

            //CustomExporter customExporter = new CustomExporter(currentDocument, new TerrainExportContext());
            //customExporter.Export(this.ActiveView);

            //currentDocument.Export("C:\\Users\\your mom\\Desktop", "terrain.stl", exportOptions);
        }

        private static void OnlyShowCategories(Document doc, View3D view, ImmutableHashSet<BuiltInCategory> showCategories)
        {
            foreach (Category cat in doc.Settings.Categories)
            {
                if (!view.CanCategoryBeHidden(cat.Id))
                {
                    continue;
                }

#if REVIT2023_OR_GREATER
                var asBuiltInCategory = cat.BuiltInCategory;
#else
                var asBuiltInCategory = (BuiltInCategory)cat.Id.IntegerValue;
#endif

                view.SetCategoryHidden(cat.Id, !showCategories.Contains(asBuiltInCategory));
            }
        }

        private static void DeleteViewIfExists(string name, Document doc)
        {
            try
            {
                View view = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Views)
                    .WhereElementIsNotElementType()
                    .Cast<View>()
                    .First(x => x.Name == name);

                doc.Delete(view.Id);
            }
            catch (InvalidOperationException)
            {
                return;
            }
        }

        private static View3D CreateView(string name, Document doc, View threeDView)
        {
            DeleteViewIfExists(name, doc);

            var view = DuplicateThreeDView(doc, threeDView);

            view.Name = name;
            view.DetailLevel = ViewDetailLevel.Coarse;
            view.DisplayStyle = DisplayStyle.Shading;

            return view;
        }

        private static View3D DuplicateThreeDView(Document doc, View threeDView)
        {
            var viewId = threeDView.Duplicate(ViewDuplicateOption.Duplicate);

            if (doc.GetElement(viewId) is not View3D view)
            {
                throw new Exception("failed to duplicate 3D View");
            }

            view.ViewTemplateId = ElementId.InvalidElementId;

            return view;
        }
    }
}
