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
        public DownstreamApiHelper DownstreamApiHelper { get; }

        /// <summary>
        /// Gets the instance of MSALClientHelper.
        /// </summary>
        public MSALClientHelper MSALClientHelper { get; }

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

            DownStreamApiConfig downStreamApiConfig = AppConfiguration.GetSection("DownstreamApi").Get<DownStreamApiConfig>();
            this.DownstreamApiHelper = new DownstreamApiHelper(downStreamApiConfig, this.MSALClientHelper);
        }

        /// <summary>
        /// Acquire the token silently
        /// </summary>
        /// <returns>An access token</returns>
        public async Task<string> AcquireTokenSilentAsync()
        {
            // Get accounts by policy
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
        }

        /// <summary>
        /// It will sign out the user.
        /// </summary>
        /// <returns></returns>
        internal async Task SignOutAsync()
        {
            await this.MSALClientHelper.SignOutUserAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets scopes for the application
        /// </summary>
        /// <returns>An array of all scopes</returns>
        internal string[] GetScopes()
        {
            return this.DownstreamApiHelper.DownstreamApiConfig.ScopesArray;
        }
    }
}