// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using Microsoft.Identity.Client;

namespace MauiB2C.MSALClient
{
    /// <summary>
    /// This is a wrapper for PublicClientApplication. It is singleton and can be utilized by both application and the MAM callback
    /// </summary>    
    public class PCAWrapperB2C
    {
        /// <summary>
        /// This is the singleton used by consumers. Since PCAWrapper constructor does not have perf or memory issue, it is instantiated directly.
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
                                        .WithLogging(LogHere)
                                        .WithB2CAuthority(B2CConstants.AuthoritySignInSignUp)
                                        .WithIosKeychainSecurityGroup(B2CConstants.IOSKeyChainGroup)
                                        .WithRedirectUri(B2CConstants.RedirectUri)
                                        .Build();
        }

        // This demos logging
        private void LogHere(LogLevel level, string message, bool containsPii)
        {
            // You can do customized logging here. This is just for demo.
            Debug.WriteLine(message);
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
        /// Signout may not perform the complete signout as company portal may hold
        /// the token.
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
    }
}
