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
        /// This encapuslates the common pattern to acquire token i.e. attempt AcquireTokenSilent and if that throws MsalUiRequiredException attempt interactively,
        /// Interactive attempt is optional.
        /// If AcquireTokenInteractiveParameterBuilder needs to be cusomized prior to the execution, it provides a delegate.
        /// </summary>
        /// <param name="doSilent">Attempts silent acquire based on the value</param>
        /// <param name="doInteractive">UI interaction even if silent action fails<</param>
        /// <param name="preferredAccount">Function that determines th account to be used. The default is first. (optional)</param>
        /// <param name="customizeSilent">This is a delegate to optionally customize AcquireTokenSilentParameterBuilder prior to execute</param>
        /// <param name="customizeInteractive">This is a delegate to optionally customize AcquireTokenInteractiveParameterBuilder prior to execute</param>
        /// <returns></returns>
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
        /// This will add bearer token to request message from the Authentication result
        /// </summary>
        /// <param name="message"></param>
        void AddAuthenticationBearerToken(HttpRequestMessage message);
    }
}
