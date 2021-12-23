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
    /// 2. EnsureAuthenticatedAsync has delegates to customize AcquireTokenInteractiveParameterBuilder, AcquireTokenSilentParameterBuilder and selection of account
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

        // desired scopes
        private string[] _scopes;

        /// <summary>
        /// Initializes the PCAHelpr or its derived class as per the generics and PublicClientApplicationBuilder with the given parameters
        /// PublicClientApplicationBuilder can be customized after this method prior to accessing PublicClientApplication.
        /// By default it is singleton pattern with option to force creation. 
        /// </summary>
        /// <typeparam name="T">Any class that is inherited from PCAHelper</typeparam>
        /// <param name="clientId">Client id of your application</param>
        /// <param name="scopes">The desired scope</param>
        /// <param name="specialRedirectUri">If you are using recommended pattern fo rredirect Uri, this is optional</param>
        /// <param name="forceCreate">Creates a new instance irrespective of the existance of the previous instance</param>
        /// <returns>Instance of class inherited from PCAHelper</returns>
        public static IPCAHelper Init<T>(string clientId, string[] scopes, string specialRedirectUri = null, bool forceCreate = false) 
                where T : PCAHelper, new()
        {
            if (Instance == null || forceCreate)
            {
                var pcaHelper = new T();
                pcaHelper._scopes = scopes;
                pcaHelper.PCABuilder = PublicClientApplicationBuilder.Create(clientId)
                                                                    .WithRedirectUri(specialRedirectUri ?? $"msal{clientId}://auth")
                                                                    .WithIosKeychainSecurityGroup("com.microsoft.adalcache");
                Instance = pcaHelper;
            }
            return Instance;
        }

        /// <summary>
        /// This encapuslates the common pattern to acquire token i.e. attempt AcquireTokenSilent and if that throws MsalUiRequiredException attempt acquire token interactively.
        /// Interactive attempt is optional.
        /// It provides optional delegates to customize behavior.
        /// </summary>
        /// <param name="doSilent">Determines whether to execute AcquireTokenSilent</param>
        /// <param name="doInteractive">Determines whether to execute AcquireTokenInteractive. By detault, UI interaction takes place if silent action fails.</param>
        /// <param name="preferredAccount">Function that determines the account to be used. The default is first. (optional)</param>
        /// <param name="customizeSilent">This is a delegate to optionally customize AcquireTokenSilentParameterBuilder.</param>
        /// <param name="customizeInteractive">This is a delegate to optionally customize AcquireTokenInteractiveParameterBuilder.</param>
        /// <returns>Authenitcation result</returns>
        public async Task<AuthenticationResult> EnsureAuthenticatedAsync(
                                                                bool doSilent = true,
                                                                bool doInteractive = true,
                                                                Func<IEnumerable<IAccount>, IAccount> preferredAccount = null,
                                                                Action<AcquireTokenSilentParameterBuilder> customizeSilent = null,
                                                                Action<AcquireTokenInteractiveParameterBuilder> customizeInteractive = null)
        {
            AuthResult = null;

            try
            {
                if (doSilent)
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
                    var silentparamsBuilder = PCA.AcquireTokenSilent(_scopes, account);
                    if (customizeSilent != null)
                    {
                        customizeSilent(silentparamsBuilder);
                    }

                    AuthResult = await silentparamsBuilder.ExecuteAsync()
                                          .ConfigureAwait(false);
                }
                else if (doInteractive)
                {
                    await AcquireInteractive(customizeInteractive).ConfigureAwait(false);
                }
                else
                {
                    throw new ArgumentException($"Both doSilent and do Interactive cannot be false");
                }
            }
            catch (MsalUiRequiredException)
            {
                if (doInteractive)
                {
                    await AcquireInteractive(customizeInteractive).ConfigureAwait(false);
                }
            }

            return AuthResult;
        }

        // acquire interactively.
        private async Task AcquireInteractive(Action<AcquireTokenInteractiveParameterBuilder> customizeInteractive)
        {
            try
            {
                var builder = PCA.AcquireTokenInteractive(_scopes)
                                                           .WithParentActivityOrWindow(ParentWindow);

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
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthResult.AccessToken);
        }
    }
}
