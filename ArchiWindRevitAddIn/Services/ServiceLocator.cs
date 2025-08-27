using ArchiwindRevitAddIn.Api;
using ArchiWindRevitAddIn.Views;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using System.Security;

namespace ArchiWindRevitAddIn.Services
{
    public static class ServiceLocator
    {
        private static HttpClient? _apiClient;
        private static readonly object _lock = new();

        public static HttpClient ApiClient
        {
            get
            {
                if (_apiClient == null)
                {
                    lock (_lock)
                    {
                        _apiClient ??= CreateApiClient();
                    }
                }

                return _apiClient;
            }
        }

        public static void Initialize()
        {
            lock (_lock)
            {
                _apiClient = CreateApiClient();
            }
        }

        public static HttpClient CreateApiClient(SecureString? pat = null)
        {
            return CreateApiClient(pat, GetBaseUrl());
        }

        private static HttpClient CreateApiClient(SecureString? pat, string? baseUrl)
        {
            var authProvider = new PersonalAccessTokenAuthenticationProvider(pat);

            var requestAdapter = new HttpClientRequestAdapter(authProvider);
            if (baseUrl != null)
            {
                requestAdapter.BaseUrl = baseUrl;
            }

            return new HttpClient(requestAdapter);
        }

        private static string? GetBaseUrl()
        {
            return Environment.GetEnvironmentVariable("ARCHIWIND_BASEURL");
        }

        public static void Dispose()
        {
            lock (_lock)
            {
                _apiClient = null;
            }
        }
    }

    public class PersonalAccessTokenAuthenticationProvider : IAuthenticationProvider
    {
        private readonly SecureString? pat;

        public PersonalAccessTokenAuthenticationProvider(SecureString? pat = null)
        {
            this.pat = pat;
        }

        public Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
        {
            var pat = this.pat ?? ConfigurationService.RetrievePAT();

            if (pat == null)
            {
                return Task.FromException(new InvalidOperationException("no PAT configured"));
            }

            request.Headers.TryAdd("x-nablaflow-token", Utils.ConvertSecureStringToString(pat));

            return Task.CompletedTask;
        }
    }
}
