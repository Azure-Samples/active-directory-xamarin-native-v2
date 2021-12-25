using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Identity.Client.Helper
{
    /// <summary>
    /// This is the interface for helper class to encapsulate the calling patterns used in the Public Client Applications.
    /// At the same time, developers can customize its behaviors.

    /// 1. PublicClientApplicationBuilder PCABuilder - is created in Init and is available to customize before accessing PCA.

    /// 2. EnsureAuthenticatedAsync has delegates to customize AcquireTokenInteractiveParameterBuilder, AcquireTokenSilentParameterBuilder and selection of account
    /// before excute is called.
    /// </summary>
    public interface IPCAHelper
    {
        /// <summary>
        /// IPublicClientApplication that is created on the first get
        /// If you want to customize PublicClientApplicationBuilder, please do it before calling the first get
        /// </summary>
        IPublicClientApplication PCA { get; }

        /// <summary>
        /// Application builder. It is created in Init and thie member can be used to customize it before Build occurs in PCA->get
        /// </summary>
        PublicClientApplicationBuilder PCABuilder { get; }

        /// <summary>
        /// This is applicable to Android. Please update this property in MainActivity.Create method
        /// and consequently with change in the current activity.
        /// </summary>
        object ParentWindow { get; set; }

        /// <summary>
        /// In UWP app, set it to true.
        /// </summary>
        bool IsUWP { get; set; }

        /// <summary>
        /// This stores the authentication result, from the auth process.
        /// When the process starts, it is set to null.
        /// </summary>
        AuthenticationResult AuthResult { get; }

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
        Task<AuthenticationResult> EnsureAuthenticatedAsync(
                                                                bool doSilent = true,
                                                                bool doInteractive = true,
                                                                Func<IEnumerable<IAccount>, IAccount> preferredAccount = null,
                                                                Action<AcquireTokenSilentParameterBuilder> customizeSilent = null,
                                                                Action<AcquireTokenInteractiveParameterBuilder> customizeInteractive = null);

        /// <summary>
        /// This will remove all the accounts.
        /// </summary>
        /// <returns></returns>
        Task SignOutAsync();

        /// <summary>
        /// This will add bearer token to request message as per the Authentication result.
        /// It is assumed that the class has valid AuthenticationResult
        /// </summary>
        /// <param name="message">Message that needs token</param>
        void AddAuthenticationBearerToken(HttpRequestMessage message);
    }
}
