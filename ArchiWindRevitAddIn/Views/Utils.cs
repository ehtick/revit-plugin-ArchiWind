using System.Collections.Immutable;

namespace ArchiWindRevitAddIn.Views
{
    internal class Utils
    {
        public const string BUILDING_VIEW = "ArchiWind - Building";
        public const string SURROUNDINGS_VIEW = "ArchiWind - Surroundings";
        public const string TERRAIN_VIEW = "ArchiWind - Terrain";
        public const string VEGETATION_VIEW = "ArchiWind - Vegetation";

        public static int CountElementsInView(Document doc, View3D view, ImmutableHashSet<BuiltInCategory> categories)
        {
            return categories.Select(cat =>
            {
                return new FilteredElementCollector(doc, view.Id)
                .OfCategory(cat)
                .WhereElementIsNotElementType()
                .GetElementCount();
            }).Sum();
        }

        public static void OnlyShowCategories(Document doc, View3D view, ImmutableHashSet<BuiltInCategory> showCategories)
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

        public static View? FindView(Document doc, string name)
        {
            try
            {
                return new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Views)
                    .WhereElementIsNotElementType()
                    .Cast<View>()
                    .First(x => x.Name == name);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public static void DeleteViewIfExists(Document doc, string name)
        {
            View? view = FindView(doc, name);

            if (view != null)
            {
                doc.Delete(view.Id);
            }
        }

        public static View3D CreateView(Document doc, View threeDView, string name)
        {
            DeleteViewIfExists(doc, name);

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
