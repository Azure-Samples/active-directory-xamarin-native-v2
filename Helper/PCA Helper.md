# PCA Helper
The utility provides easy to use API and flexibility for granular programming for authentication in public client application. This is achieved by encapsulating the common patterns and providing optional delegates for more granular programming. The APIs accepts have various commonly used values making it configurable.

It has the following features:
- The API is interface based. Developer can create Mocks and perform testing with no dependency on MSAL.NET.

- The initialization allows specialization of the PCAHelper class should developer choose it.
- It is Singleton by default, it can be overridden.
- API provides commonly used values as defaults, reducing the burden on the developers.
- It provides delegates for customization allowing granular programming.

# APIs
The APIs are briefly described here. Developer will first need to initialize the utility and then call API to acquire token.

## Initialization
Initialization can be done in the App class. It instantiates the helper with the client id and various optional parameters. By default, the PCAHelper is a singleton. It can be overwritten by forcing creation. Other actions such as logging, token cache initialization can be done optionally in the postInit action.

```CSharp

        /// <summary>
        /// Instantiates the PCAHelper (or its derived class) and PublicClientApplicationBuilder with the given parameters.
        /// PublicClientApplicationBuilder can be customized after this method or in the postInit prior to accessing PublicClientApplication.
        /// By default it is singleton pattern with option to force creation. 
        /// </summary>
        /// <typeparam name="T">Any class that is inherited from PCAHelper</typeparam>
        /// <param name="clientId">Client id of your application</param>
        /// <param name="specialRedirectUri">If you are using recommended pattern for redirect Uri (i.e. $"msal{clientId}://auth"), this is optional</param>
        /// <param name="authority">Authority to acquire token. If this is supplied with tenantID, tenantID need not be supplied as parameter. </param>
        /// <param name="tenantId">TenantID - This is required for single tenant app.</param>
        /// <param name="useBroker">To use broker or not. Recommended practice is to use it for security.</param>
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
```

Example usage:
```CSharp
// initialize with the client id and redirectURI. One can optionally pass the other parameters
PCAHelper.Init<PCAHelper>(App.ClientID, redirectUri);
// additional customization of the builder
PCAHelper.Instance.PCABuilder.WithB2CAuthority(B2CConstants.AuthoritySignInSignUp);
```

## Acquire the token
AcquireTokenAsync will acquire authentication token given the scopes. It attempts to acquire the token silently and if that fails, it presents an interactive sign-in dialogue to the user. Developer can provide tenantID in case of multi-tenant application. It also has optional delegates to select the preferred account, customize silent and interactive acquisition.

``` CSharp
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
        Task<AuthenticationResult> AcquireTokenAsync(
                                                             string[] scopes,
                                                             string tenantID = null,
                                                             Func<IEnumerable<IAccount>, IAccount> preferredAccount = null,
                                                             Action<AcquireTokenSilentParameterBuilder> customizeSilent = null,
                                                             Action<AcquireTokenInteractiveParameterBuilder> customizeInteractive = null);
```

Example usage (B2C sample):
``` CSharp
var authResult = PCAHelper.Instance.AcquireTokenAsync(App.Scopes, 
                                     preferredAccount:(accounts) => GetAccountByPolicy(accounts, B2CConstants.PolicyEditProfile),
                                     customizeInteractive: (builder) =>
                                                {
                                                    builder.WithPrompt(Prompt.NoPrompt)
                                                           .WithAuthority(B2CConstantsAuthorityEditProfile);
                                                }).ConfigureAwait(false);
```

## Use the token
User can sign a request without having to deal with the token.

``` CSharp
public void AddAuthenticationBearerToken(HttpRequestMessage message)
```

## Sign out
Here is the API to sign out

``` CSharp
public async Task SignOutAsync()
```

# Properties
Apart from the above, the API provides Instance of the PCHelper that has properties to do more granular programming:

``` CSharp

        /// <summary>
        /// IPublicClientApplication that is created on the first get
        /// If you want to customize PublicClientApplicationBuilder, please do it before calling the first get
        /// </summary>
        IPublicClientApplication PCA { get; }

        /// <summary>
        /// Application builder. It is created in Init and this member can be used to customize it before Build occurs in PCA->get
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
```