using ArchiWindRevitAddIn.Exceptions;
using Autodesk.Revit.UI;
using System.IO;

namespace ArchiWindRevitAddIn.ExternalEventHandlers
{
    public class STLExportParams
    {
        public required string Folder { get; set; }
        public required string Filename { get; set; }
        public required STLExportOptions ExportOptions { get; set; }
    }

    public class STLExportHandler : IExternalEventHandler
    {
        public STLExportParams? ExportParams { get; set; }
        public TaskCompletionSource<string>? TaskCompletion { get; set; }

        public string GetName() => "STL Export Handler";

        public void Execute(UIApplication app)
        {
            try
            {
                if (ExportParams is null)
                {
                    throw new InvalidOperationException("No ExportParams set");
                }

                Document doc = app.ActiveUIDocument.Document;

                bool result = doc.Export(ExportParams.Folder, ExportParams.Filename, ExportParams.ExportOptions);

                if (result == false)
                {
                    throw new StlExportFailed($"Failed to export {ExportParams.Filename} because not in main thread");
                }

                TaskCompletion?.SetResult(Path.Combine(ExportParams.Folder, ExportParams.Filename));
            }
            catch (Exception ex)
            {
                TaskCompletion?.SetException(ex);
            }
        }
    }
}