using System.Security;

namespace ArchiWindRevitAddIn.Models.Forms
{
    public class AccountSettingsForm
    {
        public SecureString Pat { get; set; } = new SecureString();
    }
}
