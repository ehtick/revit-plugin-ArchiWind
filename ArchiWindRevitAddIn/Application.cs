using ArchiWindRevitAddIn.Services;
using Nice3point.Revit.Toolkit.External;

namespace ArchiWindRevitAddIn
{
    /// <summary>
    ///     Application entry point
    /// </summary>
    public class Application : ExternalApplication
    {
        private const string TAB_NAME = "ArchiWind";

        public override void OnStartup()
        {
            InitialiseServices();

            CreateRibbon();
        }

        public override void OnShutdown()
        {
            ServiceLocator.Dispose();
        }

        private void CreateRibbon()
        {
            var viewsPanel = Application.CreatePanel("3D Views", TAB_NAME);

            viewsPanel.AddPushButton<Commands.PreviewGeometries>("Create")
                .SetImage("/ArchiWindRevitAddIn;component/Resources/Icons/create-views-light@16.png")
                .SetLargeImage("/ArchiWindRevitAddIn;component/Resources/Icons/create-views-light@32.png");

            viewsPanel.AddPushButton<Commands.ExportStls>("Export STLs")
                .SetImage("/ArchiWindRevitAddIn;component/Resources/Icons/export-stls-light@16.png")
                .SetLargeImage("/ArchiWindRevitAddIn;component/Resources/Icons/export-stls-light@32.png");

            var commandsPanel = Application.CreatePanel("Simulation", TAB_NAME);

            commandsPanel.AddPushButton<Commands.CreateSimulation>("Create")
                .SetImage("/ArchiWindRevitAddIn;component/Resources/Icons/create-simulation-light@16.png")
                .SetLargeImage("/ArchiWindRevitAddIn;component/Resources/Icons/create-simulation-light@32.png");

            var settingsPanel = Application.CreatePanel("Settings", TAB_NAME);

            settingsPanel.AddPushButton<Commands.AccountSettings>("Account")
                .SetImage("/ArchiWindRevitAddIn;component/Resources/Icons/user-settings-light@16.png")
                .SetLargeImage("/ArchiWindRevitAddIn;component/Resources/Icons/user-settings-light@32.png");
        }

        private static void InitialiseServices()
        {
            try
            {
                ServiceLocator.Initialize();
                System.Diagnostics.Debug.WriteLine("Initialised API services");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to initialise API services: {ex.Message}");
            }
        }
    }
}
