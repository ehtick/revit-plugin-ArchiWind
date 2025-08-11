using ArchiWindRevitAddIn.Commands;
using Nice3point.Revit.Toolkit.External;

namespace ArchiWindRevitAddIn
{
    /// <summary>
    ///     Application entry point
    /// </summary>
    [UsedImplicitly]
    public class Application : ExternalApplication
    {
        public override void OnStartup()
        {
            CreateRibbon();
        }

        private void CreateRibbon()
        {
            var panel = Application.CreatePanel("Commands", "ArchiWind");

            panel.AddPushButton<StartupCommand>("Execute")
                .SetImage("/ArchiWindRevitAddIn;component/Resources/Icons/RibbonIcon16.png")
                .SetLargeImage("/ArchiWindRevitAddIn;component/Resources/Icons/RibbonIcon32.png");
        }
    }
}