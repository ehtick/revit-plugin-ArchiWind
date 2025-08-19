using Microsoft.Win32;

namespace ArchiWindRevitAddIn.Services
{
    public static class ConfigurationService
    {
        private const string REGISTRY_KEY = @"SOFTWARE\NablaFlow\ArchiWindForRevit";
        private const string PAT = "PAT";

        public static string? GetApiKey()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY);
                return key?.GetValue(PAT) as string;
            }
            catch
            {
                return null;
            }
        }

        public static void SetApiKey(string apiKey)
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(REGISTRY_KEY);
                key.SetValue(PAT, apiKey);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to save PAT", ex);
            }
        }
        public static void ClearConfiguration()
        {
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree(REGISTRY_KEY, false);
            }
            catch
            {
            }
        }
    }
}
