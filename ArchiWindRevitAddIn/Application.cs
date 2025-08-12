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
            CreateRibbon();
        }

        private void CreateRibbon()
        {
            var commandsPanel = Application.CreatePanel("Simulations", TAB_NAME);

            commandsPanel.AddPushButton<Commands.CreateSimulation>("Create")
                .SetImage("/ArchiWindRevitAddIn;component/Resources/Icons/RibbonIcon16.png")
                .SetLargeImage("/ArchiWindRevitAddIn;component/Resources/Icons/RibbonIcon32.png");

            var settingsPanel = Application.CreatePanel("Settings", TAB_NAME);

            settingsPanel.AddPushButton<Commands.GeometriesSettings>("Geometries")
                .SetImage("/ArchiWindRevitAddIn;component/Resources/Icons/RibbonIcon16.png")
                .SetLargeImage("/ArchiWindRevitAddIn;component/Resources/Icons/RibbonIcon32.png");

            settingsPanel.AddPushButton<Commands.AccountSettings>("Account")
                .SetImage("/ArchiWindRevitAddIn;component/Resources/Icons/RibbonIcon16.png")
                .SetLargeImage("/ArchiWindRevitAddIn;component/Resources/Icons/RibbonIcon32.png");
        }
    }
}