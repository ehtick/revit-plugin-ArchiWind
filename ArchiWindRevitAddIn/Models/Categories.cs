namespace ArchiWindRevitAddIn.Models
{
    public sealed class Categories
    {
        public static readonly HashSet<BuiltInCategory> DefaultBuildingCategories =
        [
            BuiltInCategory.OST_Ceilings,
            BuiltInCategory.OST_Curtain_Systems,
            BuiltInCategory.OST_CurtainGrids,
            BuiltInCategory.OST_CurtainWallMullions,
            BuiltInCategory.OST_CurtainWallPanels,
            BuiltInCategory.OST_Doors,
            BuiltInCategory.OST_Floors,
            BuiltInCategory.OST_Mass,
            BuiltInCategory.OST_Railings,
            BuiltInCategory.OST_Roofs,
            BuiltInCategory.OST_Stairs,
            BuiltInCategory.OST_StairsRailing,
            BuiltInCategory.OST_Walls,
            BuiltInCategory.OST_Windows,
        ];

        public static readonly HashSet<BuiltInCategory> DefaultSurroundingsCategories =
        [
            BuiltInCategory.OST_Roads,
            BuiltInCategory.OST_Site,
        ];

        public static readonly HashSet<BuiltInCategory> DefaultTerrainCategories =
        [
            BuiltInCategory.OST_Topography,
#if REVIT2023_OR_GREATER
            BuiltInCategory.OST_Toposolid,
#endif
        ];

        public static readonly HashSet<BuiltInCategory> DefaultVegetationCategories =
        [
            BuiltInCategory.OST_Planting,
        ];
    }
}
