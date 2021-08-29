using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Identity.Client.Helper
{
    /// <summary>
    /// This helper class encapsulates the common functionality used in the Apps
    /// At the same time, developers can customizes its behavior in two places
    /// 1. PublicClientApplicationBuilder PCABuilder - is created in Init and is available to customizes before accessing PCA
    /// 2. EnsureAuhenticated This has two optional delegates one tocustomize the AcquireTokenInteractiveParameterBuilder and other to customize AcquireTokenSilentParameterBuilder before excute is called
    /// </summary>
    public class PCAHelper
    {
        /// <summary>
        /// Instance of the helper
        /// </summary>
        public static PCAHelper Instance { get; private set; }

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
        /// Application builder. It is created in Init and thie member can be used to customize it before Build occurs in PCA->get
        /// </summary>
        public PublicClientApplicationBuilder PCABuilder { get; private set; }

        /// <summary>
        /// This is applicable to Android. Please update this property in MainActivity.Create method
        /// and consequently with change in the current activity.
        /// </summary>
        public static object ParentWindow { get; set; } = null;

        /// <summary>
        /// In UWP app, set it to true.
        /// </summary>
        public static bool IsUWP { get; set; } = false;

        /// <summary>
        /// This stores the authentication result, from the auth process.
        /// When the process starts, it is set to null.
        /// </summary>
        public AuthenticationResult AuthResult { get; private set; }

        // desired scopes
        private string[] _scopes;

        // Private constructor to keep it singleton
        private PCAHelper()
        {
        }

        /// <summary>
        /// Initializes the instance and PublicClientApplicationBuilder with the give parameters
        /// PublicClientApplicationBuilder can be customized after the call.
        /// </summary>
        /// <param name="clientId">Client id of your application</param>
        /// <param name="scopes">The desired scope</param>
        /// <param name="specialRedirectUri">If you are using recommended pattern fo rredirect Uri, this is optional</param>
        /// <returns></returns>
        public static PCAHelper Init(string clientId, string[] scopes, string specialRedirectUri = null)
        {
            if (Instance == null)
            {
                Instance = new PCAHelper();
                Instance._scopes = scopes;
                Instance.PCABuilder = PublicClientApplicationBuilder.Create(clientId)
                                                                    .WithRedirectUri(specialRedirectUri ?? $"msal{clientId}://auth")
                                                                    .WithIosKeychainSecurityGroup("com.microsoft.adalcache");
            }
            return Instance;
        }

        /// <summary>
        /// This encapuslates the common pattern to acquire token i.e. attempt AcquireTokenSilent and if that throws MsalUiRequiredException attempt interactively,
        /// Interactive attempt is optional.
        /// If AcquireTokenInteractiveParameterBuilder needs to be cusomized prior to the execution, it provides a delegate.
        /// </summary>
        /// <param name="silentOnly">If true, does not attempt UI interaction even if silent action fails</param>
        /// <param name="account">Account to be used. (optional)</param>
        /// <param name="customizeSilent">This is a delegate to optionally customize AcquireTokenSilentParameterBuilder prior to execute</param>
        /// <param name="customizeInteractive">This is a delegate to optionally customize AcquireTokenInteractiveParameterBuilder prior to execute</param>
        /// <returns></returns>
        public async Task<AuthenticationResult> EnsureAuthenticatedAsync(bool silentOnly = false, IAccount account = null, Action<AcquireTokenSilentParameterBuilder> customizeSilent = null, Action<AcquireTokenInteractiveParameterBuilder> customizeInteractive = null)
        {
            AuthResult = null;

            try
            {
                // Customize silentBuilder
                var silentparamsBuilder = PCA.AcquireTokenSilent(_scopes, account);
                if (customizeSilent != null)
                {
                    customizeSilent(silentparamsBuilder);
                }
                
                AuthResult = await silentparamsBuilder.ExecuteAsync()
                                      .ConfigureAwait(false);
            }
            catch (MsalUiRequiredException)
            {
                if (!silentOnly)
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
            }

            return AuthResult;
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
        /// This will add bearer token to request message from the Authentication result
        /// </summary>
        /// <param name="message"></param>
        public void AddAuthenticationBearerToken(HttpRequestMessage message)
        {
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthResult.AccessToken);
        }
    }
}
