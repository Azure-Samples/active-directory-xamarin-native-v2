// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using Microsoft.Identity.Client;
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
        /// Instance of PublicClientApplication. It is provided, if App wants more customization.
        /// </summary>
        internal IPublicClientApplication PCA { get; }

        // private constructor for singleton
        private PCAWrapperB2C()
        {
            // Create PCA once. Make sure that all the config parameters below are passed
            PCA = PublicClientApplicationBuilder
                                        .Create(B2CConstants.ClientID)
                                        .WithExperimentalFeatures() // this is for upcoming logger
                                        .WithLogging(_logger)
                                        .WithB2CAuthority(B2CConstants.AuthoritySignInSignUp)
                                        .WithIosKeychainSecurityGroup(B2CConstants.IOSKeyChainGroup)
                                        .WithRedirectUri(B2CConstants.RedirectUri)
                                        .Build();
        }

        /// <summary>
        /// Acquire the token silently
        /// </summary>
        /// <param name="scopes">desired scopes</param>
        /// <returns>Authentication result</returns>
        public async Task<AuthenticationResult> AcquireTokenSilentAsync(string[] scopes)
        {
            // Get accounts by policy
            IEnumerable<IAccount> accounts = await PCA.GetAccountsAsync(B2CConstants.PolicySignUpSignIn).ConfigureAwait(false);

            AuthenticationResult authResult = await PCA.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
               .WithB2CAuthority(B2CConstants.AuthoritySignInSignUp)
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
            return await PCA.AcquireTokenInteractive(B2CConstants.Scopes)
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
