// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.IdentityModel.Abstractions;
using System.Reflection;

namespace MauiAppBasic.MSALClient
{
    /// <summary>
    /// This is a wrapper for PublicClientApplication. It is singleton.
    /// </summary>
    public class PCAWrapper
    {
        /// <summary>
        /// This is the singleton used by Ux. Since PCAWrapper constructor does not have perf or memory issue, it is instantiated directly.
        /// </summary>
        public static PCAWrapper Instance { get; private set; } = new PCAWrapper();

        /// <summary>
        /// This is the configuration for the application found within the 'appsettings.json' file.
        /// </summary>
        public static IConfiguration AppConfiguration { get; private set; }

        /// <summary>
        /// Instance of PublicClientApplication. It is provided, if App wants more customization.
        /// </summary>
        internal IPublicClientApplication PCA { get; }

        /// <summary>
        /// This will determine if the Interactive Authentication should be Embedded or System view
        /// </summary>
        internal bool UseEmbedded { get; set; } = false;

        /// <summary>
        /// Cache helper for WinUI applications.
        /// </summary>
        private MsalCacheHelper MsalCacheHelper { get; set; }

        // private constructor for singleton
        private PCAWrapper()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("MauiAppBasic.appsettings.json");
            AppConfiguration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            // Create PCA once. Make sure that all the config parameters below are passed
            PCA = PublicClientApplicationBuilder
                                        .Create(AppConfiguration["AzureAd:ClientId"])
                                        .WithAuthority($"{AppConfiguration["AzureAd:Instance"]}{AppConfiguration["AzureAd:TenantId"]}")
                                        .WithExperimentalFeatures() // this is for upcoming logger
                                        .WithLogging(_logger)
                                        .WithRedirectUri(PlatformConfig.Instance.RedirectUri)
                                        .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                                        .Build();

        }

        /// <summary>
        /// Acquire the token silently
        /// </summary>
        /// <param name="scopes">desired scopes</param>
        /// <returns>Authentication result</returns>
        internal async Task<AuthenticationResult> AcquireTokenSilentAsync()
        {
            var scopes = AppConfiguration["DownstreamApi:Scopes"].Split(" ");

            var accts = await PCA.GetAccountsAsync().ConfigureAwait(false);
            var acct = accts.FirstOrDefault();

            var authResult = await PCA.AcquireTokenSilent(scopes, acct)
                                      .ExecuteAsync().ConfigureAwait(false);
            return authResult;
        }

        /// <summary>
        /// Perform the interactive acquisition of the token for the given scope
        /// </summary>
        /// <param name="scopes">desired scopes</param>
        /// <returns></returns>
        internal async Task<AuthenticationResult> AcquireTokenInteractiveAsync()
        {
            var scopes = AppConfiguration["DownstreamApi:Scopes"].Split(" ");

            if (UseEmbedded)
            {
                return await PCA.AcquireTokenInteractive(scopes)
                                        .WithUseEmbeddedWebView(true)
                                        .WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
                                        .ExecuteAsync()
                                        .ConfigureAwait(false);
            }

            SystemWebViewOptions systemWebViewOptions = new SystemWebViewOptions();
#if IOS
            // Hide the privacy prompt in iOS
            systemWebViewOptions.iOSHidePrivacyPrompt = true;
#endif

            return await PCA.AcquireTokenInteractive(scopes)
                                    .WithSystemWebViewOptions(systemWebViewOptions)
                                    .WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
                                    .ExecuteAsync()
                                    .ConfigureAwait(false);
        }

        /// <summary>
        /// It will sign out the user.
        /// </summary>
        /// <returns></returns>
        internal async Task SignOutAsync()
        {
            var accounts = await PCA.GetAccountsAsync().ConfigureAwait(false);
            foreach (var acct in accounts)
            {
                await PCA.RemoveAsync(acct).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Ensure the cache is intialized. Only relevant for WinUI applications.
        /// </summary>
        /// <returns>Task resolving to 'true' if this is a non WinUI application otherwise 'true' after the cahce is initalized</returns>
        internal async Task<bool> InitializCache()
        {
            if (DeviceInfo.Current.Platform != DevicePlatform.WinUI || MsalCacheHelper is not null)
            {
                return true;
            }

            //Cache configuration and hook-up to public application. Refer to https://github.com/AzureAD/microsoft-authentication-extensions-for-dotnet/wiki/Cross-platform-Token-Cache#configuring-the-token-cache
            var storageCreationProperties = new StorageCreationPropertiesBuilder(AppConfiguration["CacheFileName"], AppConfiguration["CacheDir"]).Build();

            MsalCacheHelper = await MsalCacheHelper.CreateAsync(storageCreationProperties);
            MsalCacheHelper.RegisterCache(PCAWrapper.Instance.PCA.UserTokenCache);

            return true;
        }

        // Custom logger for sample
        private readonly MyLogger _logger = new MyLogger();

        // Custom logger class
        private class MyLogger : IIdentityLogger
        {
            /// <summary>
            /// Checks if log is enabled or not based on the Entry level
            /// </summary>
            /// <param name="eventLogLevel"></param>
            /// <returns></returns>
            public bool IsEnabled(EventLogLevel eventLogLevel)
            {
                //Try to pull the log level from an environment variable
                var msalEnvLogLevel = Environment.GetEnvironmentVariable("MSAL_LOG_LEVEL");

                EventLogLevel envLogLevel = EventLogLevel.Informational;
                Enum.TryParse<EventLogLevel>(msalEnvLogLevel, out envLogLevel);

                return envLogLevel <= eventLogLevel;
            }

            /// <summary>
            /// Log to console for demo purpose
            /// </summary>
            /// <param name="entry">Log Entry values</param>
            public void Log(LogEntry entry)
            {
                Console.WriteLine(entry.Message);
            }
        }
    }
}