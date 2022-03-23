// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Xamarin.Forms;

namespace activedirectoryxamarinintune
{
    /// <summary>
    /// This is a wrapper for PCA. It is singleton and can be utilized by both application and the MAM callback
    /// </summary>
    public class PCAWrapper
    {

        /// <summary>
        /// This is the singleton used by consumers
        /// </summary>
        static public PCAWrapper Instance { get; }

        internal IPublicClientApplication PCA { get; }

        // This event coordinates between different callbacks
        public static ManualResetEvent MAMRegsiteredEvent { get; } = new ManualResetEvent(false);

        /// <summary>
        /// The authority for the MSAL PublicClientApplication. Sign in will use this URL.
        /// </summary>
        private const string _authority = "https://login.microsoftonline.com/organizations";

        // ClientID of the application in (msidlab4.com)
        private static string _clientID = "94996db4-9d57-422d-a707-84a1328a3cb8"; // TODO - Replace with your client Id. And also replace in the AndroidManifest.xml

        // TenantID of the organization (msidlab4.com)
        private static string _tenantID = "f645ad92-e38d-4d1a-b510-d1b09a74a8ca"; // TODO - Replace with your TenantID. And also replace in the AndroidManifest.xml

        static string[] clientCapabilities = { "ProtApp" }; // It is must to have these capabilities

        // private constructor for singleton
        private PCAWrapper()
        {
            // Create PCA once. Make sure that all the config parameters below are passed
            // ClientCapabilities - must have ProtApp
            PCA = PublicClientApplicationBuilder
                                        .Create(_clientID)
                                        .WithAuthority(_authority)
                                        .WithBroker()
                                        .WithClientCapabilities(clientCapabilities)
                                        .WithTenantId(_tenantID)
                                        .WithRedirectUri(PlatformConfigImpl.Instance.RedirectUri)
                                        .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                                        .Build();
        }

        /// <summary>
        /// Static constructor to instantiate PCA
        /// </summary>
        static PCAWrapper()
        {
            Instance = new PCAWrapper();
        }

        /// <summary>
        /// Perform the intractive acquistion of the token for the given scope
        /// </summary>
        /// <param name="scopes">desired scopes</param>
        /// <param name="parentWindow">Parent window</param>
        /// <returns></returns>
        internal async Task<AuthenticationResult> DoInteractiveAsync(string[] scopes)
        {
            return await PCA.AcquireTokenInteractive(scopes)
                                    .WithParentActivityOrWindow(PlatformConfigImpl.Instance.ParentWindow)
                                    .WithUseEmbeddedWebView(true)
                                    .ExecuteAsync()
                                    .ConfigureAwait(false);
        }

        /// <summary>
        /// Acquire the token silently
        /// </summary>
        /// <param name="scopes">desired scopes</param>
        /// <returns>Authenticaiton result</returns>
        public async Task<AuthenticationResult> DoSilentAsync(string[] scopes)
        {
            if (PCA == null)
            {
                return null;
            }

            var accts = await PCA.GetAccountsAsync().ConfigureAwait(false);
            var acct = accts.FirstOrDefault();

            var silentParamBuilder = PCA.AcquireTokenSilent(scopes, acct);
            var authResult = await silentParamBuilder
                                        .ExecuteAsync().ConfigureAwait(false);
            return authResult;

        }

        /// <summary>
        /// Signout may not perform the complete signout as company portal may hold
        /// the token.
        /// </summary>
        /// <returns></returns>
        internal async Task SignOut()
        {
            var accounts = await PCA.GetAccountsAsync().ConfigureAwait(false);
            while (accounts.Any())
            {
                var acct = accounts.FirstOrDefault();
                await PCA.RemoveAsync(acct).ConfigureAwait(false);
                accounts = await PCA.GetAccountsAsync().ConfigureAwait(false);
            }
        }
    }
}
