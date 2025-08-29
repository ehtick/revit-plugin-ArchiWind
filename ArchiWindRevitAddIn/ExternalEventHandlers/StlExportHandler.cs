using ArchiWindRevitAddIn.Exceptions;
using Autodesk.Revit.UI;
using System.IO;

namespace ArchiWindRevitAddIn.ExternalEventHandlers
{
    public class STLExportParams
    {
        public STLExportParams(string folder, string filename, STLExportOptions exportOptions)
        {
            Folder = folder;
            Filename = filename;
            ExportOptions = exportOptions;
        }

        public string Folder { get; private set; }
        public string Filename { get; private set; }
        public STLExportOptions ExportOptions { get; private set; }
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
                if (ExportParams is not STLExportParams params_)
                {
                    throw new InvalidOperationException("No ExportParams set");
                }

                Document doc = app.ActiveUIDocument.Document;

                bool result = doc.Export(params_.Folder, params_.Filename, params_.ExportOptions);

                if (result == false)
                {
                    throw new StlExportFailed($"Failed to export {params_.Filename} because not in main thread");
                }

                TaskCompletion?.SetResult(Path.Combine(params_.Folder, params_.Filename));
            }
            catch (Exception ex)
            {
                TaskCompletion?.SetException(ex);
            }
        }
    }
}