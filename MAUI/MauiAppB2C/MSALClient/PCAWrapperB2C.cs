// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.IdentityModel.Abstractions;

namespace MauiB2C.MSALClient
{
    /// <summary>
    /// This is a wrapper for PublicClientApplication. It is singleton.
    /// </summary>    
    public class PCAWrapperB2C
    {
        /// <summary>
        /// This is the singleton used by ux. Since PCAWrapper constructor does not have perf or memory issue, it is instantiated directly.
        /// </summary>
        public static PCAWrapperB2C Instance { get; private set; } = new PCAWrapperB2C();

        /// <summary>
        /// This is the configuration for the application found within the 'appsettings.json' file.
        /// </summary>
        public static IConfiguration AppConfiguration { get; private set; }

        /// <summary>
        /// Instance of PublicClientApplication. It is provided, if App wants more customization.
        /// </summary>
        internal IPublicClientApplication PCA { get; }

        // private constructor for singleton
        private PCAWrapperB2C()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("MauiB2C.appsettings.json");

            AppConfiguration  = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            // Create PCA once. Make sure that all the config parameters below are passed
            PCA = PublicClientApplicationBuilder
                                        .Create(AppConfiguration["AzureAdB2C:ClientId"])
                                        .WithExperimentalFeatures() // this is for upcoming logger
                                        .WithLogging(_logger)
                                        .WithB2CAuthority($"{AppConfiguration["AzureAdB2C:Instance"]}/tfp/{AppConfiguration["AzureAdB2C:Domain"]}/{AppConfiguration["AzureAdB2C:SignUpSignInPolicyId"]}")
                                        .WithIosKeychainSecurityGroup(AppConfiguration["iOSKeyChainGroup"])
                                        .WithRedirectUri($"msal{AppConfiguration["AzureAdB2C:ClientId"]}://auth")
                                        .Build();

            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                //Cache configuration and hook-up to public application. Refer to https://github.com/AzureAD/microsoft-authentication-extensions-for-dotnet/wiki/Cross-platform-Token-Cache#configuring-the-token-cache
                var storageProperties = new StorageCreationPropertiesBuilder(AppConfiguration["CacheFileName"], AppConfiguration["CacheDir"]).Build();
                MsalCacheHelper.CreateAsync(storageProperties)
                    .ContinueWith(async msalCacheHelper => (await msalCacheHelper).RegisterCache(PCA.UserTokenCache));
            }
        }

        /// <summary>
        /// Acquire the token silently
        /// </summary>
        /// <param name="scopes">desired scopes</param>
        /// <returns>Authentication result</returns>
        public async Task<AuthenticationResult> AcquireTokenSilentAsync(string[] scopes)
        {
            // Get accounts by policy
            IEnumerable<IAccount> accounts = await PCA.GetAccountsAsync(PCAWrapperB2C.AppConfiguration["AzureAdB2C:SignUpSignInPolicyId"]).ConfigureAwait(false);

            AuthenticationResult authResult = await PCA.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
               .WithB2CAuthority($"{AppConfiguration["AzureAdB2C:Instance"]}/tfp/{AppConfiguration["AzureAdB2C:Domain"]}/{AppConfiguration["AzureAdB2C:SignUpSignInPolicyId"]}")
               .ExecuteAsync()
               .ConfigureAwait(false);

            return authResult;
        }

        /// <summary>
        /// Perform the interactive acquisition of the token for the given scope
        /// </summary>
        /// <param name="scopes">desired scopes</param>
        /// <returns></returns>
        internal async Task<AuthenticationResult> AcquireTokenInteractiveAsync(string[] scopes)
        {
            return await PCA.AcquireTokenInteractive(GetScopes())
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
        /// Gets scopes for the application
        /// </summary>
        /// <returns>An array of all scopes</returns>
        internal string[] GetScopes()
        {
            return AppConfiguration["DownstreamApi:Scopes"].Split(" ");
        }



        // Custom logger for sample
        private MyLogger _logger = new MyLogger();

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
