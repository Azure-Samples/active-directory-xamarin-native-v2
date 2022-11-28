// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MAUIB2C.MSALClient
{
    /// <summary>
    /// This is a singleton implementation to wrap the MSALClient and associated classes to support static initialization model for platforms that need this.
    /// </summary>
    public class PublicClientSingleton
    {
        /// <summary>
        /// This is the singleton used by Ux. Since PublicClientWrapper constructor does not have perf or memory issue, it is instantiated directly.
        /// </summary>
        public static PublicClientSingleton Instance { get; private set; } = new PublicClientSingleton();

        /// <summary>
        /// This is the configuration for the application found within the 'appsettings.json' file.
        /// </summary>
        private static IConfiguration AppConfiguration;

        /// <summary>
        /// Gets the instance of MSALClientHelper.
        /// </summary>
        public MSALClientHelper MSALClientHelper { get; }

        ///// <summary>
        ///// Instance of PublicClientApplication. It is provided, if App wants more customization.
        ///// </summary>
        //internal IPublicClientApplication PCA { get; }

        /// <summary>
        /// This will determine if the Interactive Authentication should be Embedded or System view
        /// </summary>
        public bool UseEmbedded { get; set; } = false;

        //// Custom logger for sample
        //private readonly IdentityLogger _logger = new IdentityLogger();

        /// <summary>
        /// Prevents a default instance of the <see cref="PublicClientSingleton"/> class from being created. or a private constructor for singleton
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private PublicClientSingleton()
        {
            // Load config
            var assembly = Assembly.GetExecutingAssembly();
            string embeddedConfigfilename = $"{Assembly.GetCallingAssembly().GetName().Name}.appsettings.json";
            using var stream = assembly.GetManifestResourceStream(embeddedConfigfilename);
            AppConfiguration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            AzureADB2CConfig azureADConfig = AppConfiguration.GetSection("AzureAdB2C").Get<AzureADB2CConfig>();
            this.MSALClientHelper = new MSALClientHelper(azureADConfig);

            //// Create PCA once. Make sure that all the config parameters below are passed
            //PCA = PublicClientApplicationBuilder
            //                            .Create(AppConfiguration["AzureAd:ClientId"])
            //                            .WithTenantId(AppConfiguration["AzureAd:TenantId"])
            //                            .WithExperimentalFeatures() // this is for upcoming logger
            //                            .WithLogging(_logger)
            //                            .WithRedirectUri(PlatformConfig.Instance.RedirectUri)
            //                            .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
            //                            .Build();

            //if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            //{
            //    //Cache configuration and hook-up to public application. Refer to https://github.com/AzureAD/microsoft-authentication-extensions-for-dotnet/wiki/Cross-platform-Token-Cache#configuring-the-token-cache
            //    var storageProperties = new StorageCreationPropertiesBuilder(AppConfiguration["CacheFileName"], AppConfiguration["CacheDir"]).Build();
            //    MsalCacheHelper.CreateAsync(storageProperties)
            //        .ContinueWith(async msalCacheHelper => (await msalCacheHelper).RegisterCache(PCA.UserTokenCache));
            //}
        }

        /// <summary>
        /// Acquire the token silently
        /// </summary>
        /// <returns>An access token</returns>
        public async Task<string> AcquireTokenSilentAsync()
        {
            // Get accounts by policy
            //IEnumerable<IAccount> accounts = await PCA.GetAccountsAsync(PublicClientWrapperB2C.AppConfiguration["AzureAdB2C:SignUpSignInPolicyId"]).ConfigureAwait(false);
            return await this.AcquireTokenSilentAsync(this.GetScopes()).ConfigureAwait(false);
        }

        /// <summary>
        /// Acquire the token silently
        /// </summary>
        /// <param name="scopes">desired scopes</param>
        /// <returns>An access token</returns>
        public async Task<string> AcquireTokenSilentAsync(string[] scopes)
        {
            return await this.MSALClientHelper.SignInUserAndAcquireAccessToken(scopes).ConfigureAwait(false);

            //var accts = await PCA.GetAccountsAsync().ConfigureAwait(false);
            //var acct = accts.FirstOrDefault();

            //var authResult = await PCA.AcquireTokenSilent(scopes, acct)
            //                          .ExecuteAsync().ConfigureAwait(false);
            //return authResult;
        }

        /// <summary>
        /// Perform the interactive acquisition of the token for the given scope
        /// </summary>
        /// <param name="scopes">desired scopes</param>
        /// <returns></returns>
        internal async Task<AuthenticationResult> AcquireTokenInteractiveAsync(string[] scopes)
        {
            this.MSALClientHelper.UseEmbedded = this.UseEmbedded;
            return await this.MSALClientHelper.SignInUserInteractivelyAsync(scopes).ConfigureAwait(false);

            //            if (UseEmbedded)
            //            {
            //                return await PCA.AcquireTokenInteractive(scopes)
            //                                        .WithUseEmbeddedWebView(true)
            //                                        .WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
            //                                        .ExecuteAsync()
            //                                        .ConfigureAwait(false);
            //            }

            //            SystemWebViewOptions systemWebViewOptions = new SystemWebViewOptions();
            //#if IOS
            //            // Hide the privacy prompt in iOS
            //            systemWebViewOptions.iOSHidePrivacyPrompt = true;
            //#endif

            //            return await PCA.AcquireTokenInteractive(scopes)
            //                                    .WithSystemWebViewOptions(systemWebViewOptions)
            //                                    .WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
            //                                    .ExecuteAsync()
            //                                    .ConfigureAwait(false);
        }

        /// <summary>
        /// It will sign out the user.
        /// </summary>
        /// <returns></returns>
        internal async Task SignOutAsync()
        {
            await this.MSALClientHelper.SignOutUserAsync().ConfigureAwait(false);

            //var accounts = await PCA.GetAccountsAsync().ConfigureAwait(false);
            //foreach (var acct in accounts)
            //{
            //    await PCA.RemoveAsync(acct).ConfigureAwait(false);
            //}
        }

        /// <summary>
        /// Gets scopes for the application
        /// </summary>
        /// <returns>An array of all scopes</returns>
        internal string[] GetScopes()
        {
            return AppConfiguration["DownstreamApi:Scopes"].Split(" ");
        }

        //// Custom logger for sample
        //private readonly MyLogger _logger = new MyLogger();

        //// Custom logger class
        //private class MyLogger : IIdentityLogger
        //{
        //    /// <summary>
        //    /// Checks if log is enabled or not based on the Entry level
        //    /// </summary>
        //    /// <param name="eventLogLevel"></param>
        //    /// <returns></returns>
        //    public bool IsEnabled(EventLogLevel eventLogLevel)
        //    {
        //        //Try to pull the log level from an environment variable
        //        var msalEnvLogLevel = Environment.GetEnvironmentVariable("MSAL_LOG_LEVEL");

        //        EventLogLevel envLogLevel = EventLogLevel.Informational;
        //        Enum.TryParse<EventLogLevel>(msalEnvLogLevel, out envLogLevel);

        //        return envLogLevel <= eventLogLevel;
        //    }

        //    /// <summary>
        //    /// Log to console for demo purpose
        //    /// </summary>
        //    /// <param name="entry">Log Entry values</param>
        //    public void Log(LogEntry entry)
        //    {
        //        Console.WriteLine(entry.Message);
        //    }
        //}
    }
}