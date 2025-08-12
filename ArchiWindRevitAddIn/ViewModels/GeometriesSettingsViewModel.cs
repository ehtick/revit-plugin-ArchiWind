using System.Collections.ObjectModel;

namespace ArchiWindRevitAddIn.ViewModels
{
    public sealed class GeometriesSettingsViewModel : ObservableObject
    {
        private ObservableCollection<BuiltInCategory> buildingCategories;
        private ObservableCollection<BuiltInCategory> surroundingsCategories;
        private ObservableCollection<BuiltInCategory> terrainCategories;
        private ObservableCollection<BuiltInCategory> vegetationCategories;

        public ObservableCollection<BuiltInCategory> BuildingCategories { get => buildingCategories; set => buildingCategories = value; }
        public ObservableCollection<BuiltInCategory> SurroundingsCategories { get => surroundingsCategories; set => surroundingsCategories = value; }
        public ObservableCollection<BuiltInCategory> TerrainCategories { get => terrainCategories; set => terrainCategories = value; }
        public ObservableCollection<BuiltInCategory> VegetationCategories { get => vegetationCategories; set => vegetationCategories = value; }

        public GeometriesSettingsViewModel()
        {
            // TODO: wrap enums in order to expose a property delegating to ToLabel
            // TODO: they should be loaded from saved preferences.
            buildingCategories = new ObservableCollection<BuiltInCategory>(Models.Categories.DefaultBuildingCategories);
            surroundingsCategories = new ObservableCollection<BuiltInCategory>(Models.Categories.DefaultSurroundingsCategories);
            terrainCategories = new ObservableCollection<BuiltInCategory>(Models.Categories.DefaultTerrainCategories);
            vegetationCategories = new ObservableCollection<BuiltInCategory>(Models.Categories.DefaultVegetationCategories);
        }
    }
}
