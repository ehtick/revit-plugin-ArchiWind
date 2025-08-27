using ArchiwindRevitAddIn.Api.Models;
using System.Security;

namespace ArchiWindRevitAddIn.Services
{
    public class TokenValidator
    {
        public enum TokenStatus
        {
            Valid,
            Invalid,
            Unknown,
        }

        public static async Task<TokenStatus> IsPatValid(SecureString pat)
        {
            var client = ServiceLocator.CreateApiClient(pat);

            try
            {
                var user = await client.Users.Self.GetAsync();

                if (user is not User)
                {
                    return TokenStatus.Unknown;
                }

                return TokenStatus.Valid;
            }
            catch (JsonErrorResponse ex)
            {
                if (ex.ResponseStatusCode == 401)
                {
                    return TokenStatus.Invalid;
                }

                return TokenStatus.Unknown;
            }
        }
    }
}
