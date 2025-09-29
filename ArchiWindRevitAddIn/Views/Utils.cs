using ArchiWindRevitAddIn.Exceptions;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

using View = Autodesk.Revit.DB.View;

namespace ArchiWindRevitAddIn.Views
{
    internal class Utils
    {
        public const string BUILDING_VIEW = "ArchiWind - Building";
        public const string SURROUNDINGS_VIEW = "ArchiWind - Surroundings";
        public const string TERRAIN_VIEW = "ArchiWind - Terrain";
        public const string VEGETATION_VIEW = "ArchiWind - Vegetation";

        public static int ShownElementsCount(Document doc, View3D view)
        {
            return new FilteredElementCollector(doc, view.Id)
                .WhereElementIsNotElementType()
                .Where(e => e.Category?.IsVisibleInUI ?? false)
                .Count();
        }

        public static void OnlyShowCategories(Document doc, View3D view, HashSet<BuiltInCategory> showCategories)
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

        public static string ConvertSecureStringToString(SecureString secureString)
        {
            if (secureString == null || secureString.Length == 0)
            {
                return string.Empty;
            }

            IntPtr ptr = Marshal.SecureStringToBSTR(secureString);

            try
            {
                return Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                Marshal.ZeroFreeBSTR(ptr);
            }
        }

        public static string ExportViewAsStl(Document doc, ElementId viewId, string folder, string filename)
        {
            STLExportOptions exportOptions = new()
            {
                TargetUnit = ExportUnit.Meter,
                ExportBinary = true,
                ExportColor = false,
                ViewId = viewId,
            };

#if REVIT2023_OR_GREATER
            exportOptions.SetTessellationSettings(ExportResolution.Medium);
#endif

            bool result = doc.Export(folder, filename, exportOptions);
            var path = Path.Combine(folder, filename);

            if (result == false || !File.Exists(path))
            {
                throw new StlExportFailed();
            }

            return path;
        }

        public static string BytesToString(long byteCount)
        {
            string[] suf = ["B", "KB", "MB", "GB", "TB", "PB", "EB"];
            if (byteCount == 0)
            {
                return "0" + suf[0];
            }

            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);

            return (Math.Sign(byteCount) * num).ToString() + suf[place];
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
