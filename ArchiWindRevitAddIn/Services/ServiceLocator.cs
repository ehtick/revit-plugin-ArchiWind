using ArchiwindRevitAddIn.Api;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace ArchiWindRevitAddIn.Services
{
    public static class ServiceLocator
    {
        private static readonly string BASE_URL = "https://api.nablaflow.io/archiwind";

        private static HttpClient? _apiClient;
        private static readonly object _lock = new object();

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

        private static HttpClient CreateApiClient()
        {
            return CreateApiClient(GetApiKey(), GetBaseUrl());
        }

        private static HttpClient CreateApiClient(string? apiKey, string baseUrl)
        {
            var authProvider = CreateAuthenticationProvider(apiKey);

            var requestAdapter = new HttpClientRequestAdapter(authProvider);
            requestAdapter.BaseUrl = baseUrl;

            return new HttpClient(requestAdapter);
        }

        private static string GetBaseUrl()
        {
            return Environment.GetEnvironmentVariable("ARCHIWIND_BASEURL") ?? BASE_URL;
        }

        private static IAuthenticationProvider CreateAuthenticationProvider(string? apiKey = null)
        {
            if (!string.IsNullOrEmpty(apiKey))
            {
                return new ApiKeyAuthenticationProvider(apiKey!);
            }

            return new AnonymousAuthenticationProvider();
        }

        private static string? GetApiKey()
        {
            return GetApiKeyFromSettings() ?? GetApiKeyFromEnvironment();
        }

        private static string? GetApiKeyFromSettings()
        {
            return ConfigurationService.GetApiKey();
        }

        private static string? GetApiKeyFromEnvironment()
        {
            return Environment.GetEnvironmentVariable("ARCHIWIND_PAT");
        }

        public static void Dispose()
        {
            lock (_lock)
            {
                _apiClient = null;
            }
        }
    }

    public class ApiKeyAuthenticationProvider : IAuthenticationProvider
    {
        private readonly string _apiKey;

        public ApiKeyAuthenticationProvider(string apiKey)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
        {
            request.Headers.TryAdd("x-nablaflow-token", _apiKey);

            return Task.CompletedTask;
        }
    }

    public class AnonymousAuthenticationProvider : IAuthenticationProvider
    {
        public Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
