using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Identity.Client.Helper
{
    /// <summary>
    /// This helper class encapsulates the calling patterns used in the Public Client Applications.
    /// At the same time, developers can customizes its behaviors
    /// 1. PublicClientApplicationBuilder PCABuilder - is created in Init and is available to customizes before accessing PCA
    /// 2. EnsureAuthenticatedAsync has delegates to customize AcquireTokenInteractiveParameterBuilder,
    /// AcquireTokenSilentParameterBuilder and selection of account
    /// before excute is called.
    /// </summary>
    public class PCAHelper : IPCAHelper
    {
        /// <summary>
        /// Instance of the helper
        /// </summary>
        public static IPCAHelper Instance { get; private set; }

        /// <summary>
        /// IPublicClientApplication that is created on the first get
        /// If you want to customize PublicClientApplicationBuilder, please do it before calling the first get
        /// </summary>
        public IPublicClientApplication PCA
        {
            get
            {
                if (_pca == null)
                {
                    _pca = PCABuilder.Build();
                }

                return _pca;
            }
        }

        // Instance of IPublicClientApplication
        private IPublicClientApplication _pca;

        /// <summary>
        /// Application builder. It is created in Init and this member can be customized before Build occurs in PCA->get
        /// </summary>
        public PublicClientApplicationBuilder PCABuilder { get; private set; }

        /// <summary>
        /// This is applicable to Android. Please update this property in MainActivity.Create method
        /// and consequently with change in the current activity.
        /// </summary>
        public object ParentWindow { get; set; } = null;

        /// <summary>
        /// In UWP app, set it to true.
        /// </summary>
        public bool IsUWP { get; set; } = false;

        /// <summary>
        /// This stores the authentication result, from the auth process.
        /// When the process starts, it is set to null.
        /// </summary>
        public AuthenticationResult AuthResult { get; internal set; }

        /// <summary>
        /// Instantiates the PCAHelpr (or its derived class) and PublicClientApplicationBuilder with the given parameters.
        /// PublicClientApplicationBuilder can be customized after this method or in the postInit prior to accessing PublicClientApplication.
        /// By default it is singleton pattern with option to force creation. 
        /// </summary>
        /// <typeparam name="T">Any class that is inherited from PCAHelper</typeparam>
        /// <param name="clientId">Client id of your application</param>
        /// <param name="specialRedirectUri">If you are using recommended pattern for redirect Uri (i.e. $"msal{clientId}://auth"), this is optional</param>
        /// <param name="authority">Authority to acquire token. If this is supplied with tenantID, tenantID need not be supplied as parameter. </param>
        /// <param name="tenantId">TenantID - This is required for single tenant app.</param>
        /// <param name="useBroker">To use broker or not. Recommended practice is to use for security.</param>
        /// <param name="postInit">Perform customization after the creation and before execute.</param>
        /// <param name="forceCreate">Creates a new instance irrespective of the existance of the previous instance</param>
        /// <returns>Instance of class inherited from PCAHelper</returns>
        public static IPCAHelper Init<T>(string clientId,
                                         string specialRedirectUri = null,
                                         string authority = null,
                                         string tenantId = null,
                                         bool useBroker = true,
                                         Action<T> postInit = null,
                                         bool forceCreate = false) 
                where T : PCAHelper, new()
        {
            if (Instance == null || forceCreate)
            {
                var pcaHelper = new T();
                pcaHelper.PCABuilder = PublicClientApplicationBuilder.Create(clientId)
                                                                    .WithRedirectUri(specialRedirectUri ?? $"msal{clientId}://auth")
                                                                    .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                                                                    .WithBroker(useBroker);
                if (!string.IsNullOrEmpty(tenantId))
                {
                    pcaHelper.PCABuilder.WithTenantId(tenantId);
                }

                if (!string.IsNullOrEmpty(authority))
                {
                    pcaHelper.PCABuilder.WithAuthority(authority);
                }

                if (postInit != null)
                {
                    postInit(pcaHelper);
                }

                Instance = pcaHelper;
            }
            return Instance;
        }

        /// <summary>
        /// This encapuslates the common pattern to acquire token i.e. attempt AcquireTokenSilent and if that throws MsalUiRequiredException 
        /// attempt acquire token interactively.
        /// It provides optional delegates to customize behavior.
        /// </summary>
        /// <param name="scopes">The desired scope</param>
        /// <param name="tenantID">TenantID for the token request in case of Multi tenant app</param>
        /// <param name="preferredAccount">Function that determines the account to be used. The default is first. (optional)</param>
        /// <param name="customizeSilent">This is a delegate to optionally customize AcquireTokenSilentParameterBuilder.</param>
        /// <param name="customizeInteractive">This is a delegate to optionally customize AcquireTokenInteractiveParameterBuilder.</param>
        /// <returns>Authentication result</returns>
        public async Task<AuthenticationResult> AcquireTokenAsync(
                                                                string[] scopes,
                                                                string tenantID = null,
                                                                Func<IEnumerable<IAccount>, IAccount> preferredAccount = null,
                                                                Action<AcquireTokenSilentParameterBuilder> customizeSilent = null,
                                                                Action<AcquireTokenInteractiveParameterBuilder> customizeInteractive = null)
        {
            AuthResult = null;

            try
            {
                IAccount account = null;
                IEnumerable<IAccount> accounts = await PCA.GetAccountsAsync().ConfigureAwait(false);
                if (preferredAccount != null)
                {
                    account = preferredAccount(accounts);
                }
                else if(accounts != null)
                {
                    account = accounts.FirstOrDefault();
                }

                // Customize silentBuilder
                var silentparamsBuilder = PCA.AcquireTokenSilent(scopes, account);
                if (tenantID != null)
                {
                    silentparamsBuilder.WithTenantId(tenantID);
                }

                if (customizeSilent != null)
                {
                    customizeSilent(silentparamsBuilder);
                }

                AuthResult = await silentparamsBuilder.ExecuteAsync()
                                        .ConfigureAwait(false);
            }
            catch (MsalUiRequiredException)
            {
                await AcquireInteractive(scopes, tenantID, customizeInteractive).ConfigureAwait(false);
            }

            return AuthResult;
        }

        // acquire interactively.
        private async Task AcquireInteractive(string[] scopes,
                                              string tenantID,
                                              Action<AcquireTokenInteractiveParameterBuilder> customizeInteractive)
        {
            try
            {
                var builder = PCA.AcquireTokenInteractive(scopes)
                                                           .WithParentActivityOrWindow(ParentWindow);
                if (tenantID != null)
                {
                    builder.WithTenantId(tenantID);
                }

                if (!IsUWP)
                {
                    // on Android and iOS, prefer to use the system browser, which does not exist on UWP
                    SystemWebViewOptions systemWebViewOptions = new SystemWebViewOptions()
                    {
                        iOSHidePrivacyPrompt = true,
                    };

                    builder.WithSystemWebViewOptions(systemWebViewOptions);
                    builder.WithUseEmbeddedWebView(false);
                }

                if (customizeInteractive != null)
                {
                    customizeInteractive(builder);
                }

                AuthResult = await builder.ExecuteAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// This will remove all the accounts.
        /// </summary>
        /// <returns></returns>
        public async Task SignOutAsync()
        {
            IEnumerable<IAccount> accounts = await PCA.GetAccountsAsync().ConfigureAwait(false);

            while (accounts.Any())
            {
                await PCA.RemoveAsync(accounts.FirstOrDefault()).ConfigureAwait(false);
                accounts = await PCA.GetAccountsAsync().ConfigureAwait(false);
            }

            AuthResult = null;
        }

        /// <summary>
        /// This will add bearer token to request message as per the Authentication result.
        /// It is assumed that the class has valid AuthenticationResult
        /// </summary>
        /// <param name="message">Message that needs token</param>
        public void AddAuthenticationBearerToken(HttpRequestMessage message)
        {
            message.Headers.Add("Authorization", AuthResult.CreateAuthorizationHeader());
        }
    }
}
