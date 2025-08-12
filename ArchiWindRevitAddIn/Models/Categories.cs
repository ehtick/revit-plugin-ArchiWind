namespace ArchiWindRevitAddIn.Models
{
    public sealed class Categories
    {
        public static readonly IEnumerable<BuiltInCategory> DefaultBuildingCategories =
        [
            BuiltInCategory.OST_Floors,
            BuiltInCategory.OST_Doors,
            BuiltInCategory.OST_Ceilings,
            BuiltInCategory.OST_Roofs,
            BuiltInCategory.OST_Windows,
            BuiltInCategory.OST_Mass,
            BuiltInCategory.OST_Railings,
            BuiltInCategory.OST_Walls,
            BuiltInCategory.OST_Stairs,
        ];

        public static readonly IEnumerable<BuiltInCategory> DefaultSurroundingsCategories =
        [
            BuiltInCategory.OST_Site
        ];

        public static readonly IEnumerable<BuiltInCategory> DefaultTerrainCategories =
        [
            BuiltInCategory.OST_Topography
        ];

        public static readonly IEnumerable<BuiltInCategory> DefaultVegetationCategories =
        [
            BuiltInCategory.OST_Planting
        ];
    }
}
