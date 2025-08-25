using System.Diagnostics;

namespace ArchiWindRevitAddIn.Models
{
    public class WSG84
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public static WSG84? FromDocument(Document document)
        {
            try
            {
                var siteLocation = document.ActiveProjectLocation?.GetSiteLocation();

                if (siteLocation == null)
                {
                    return null;
                }

                var latitude = RadiansToDegrees(siteLocation.Latitude);
                var longitude = RadiansToDegrees(siteLocation.Longitude);

                return new WSG84
                {
                    Latitude = latitude,
                    Longitude = longitude
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error extracting GPS coordinates: {ex.Message}");

                return null;
            }
        }
        private static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }
    }
}
