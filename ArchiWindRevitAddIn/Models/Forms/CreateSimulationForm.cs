using ArchiwindRevitAddIn.Api.Models;

namespace ArchiWindRevitAddIn.Models.Forms
{
    public class CreateSimulationForm
    {
        public Guid ProjectId { get; set; } = Guid.Empty;
        public string Name { get; set; } = string.Empty;
        public SimulationQuality Quality { get; set; }
        public double Latitude { get; set; } = 0.0;
        public double Longitude { get; set; } = 0.0;
        public int? RefSystem { get; set; } = null;

        public bool HasBuilding { get; set; } = false;
        public bool HasSurroundings { get; set; } = false;
        public bool HasTerrain { get; set; } = false;
        public bool HasVegetation { get; set; } = false;
    }
}
