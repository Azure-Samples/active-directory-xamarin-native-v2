# PCA Helper
PCA developers use common pattern to acquire token. The code appears to be repeatative and very granular. At the same time, the current API is based on the builder pattern. There are several "With" APIs and a some are used in commonly in the Public Client Application (PCA). Due to the abudance of the With APIs, the learning curve can be high to perform simple/common tasks.

PCAHelper extracts API at a higher level offering flexibility for granular programming as desired. The helper provides methods with optional paramaeters and lambdas for customization. It has the following features:

- It comes with two main common practices as default:
    - There is only one instance of PCA
    - Scope for permissions is defined only once.
    The above default can be customized as desired.
 - It is inteface based. So the calling app can create Mocks and perform testing w/o havinng dependency on MSAL.NET and any network connectivity
 - It allows customization of token acqusition methods.
 - It allows specialization of the Helper class should developer choose it.

# APIs
The APIs are briefly described here.

## Initialization
This can be done once in the App. This initializes the helper with client id and scope, if it does not have a standard redirect URI, it can be customized here. By default the PCAHelper is a singleton. It can be overwritten by forcing creation.

```CSharp
public static IPCAHelper Init<T>(string clientId, string[] scopes, string specialRedirectUri = null, bool forceCreate = false) 
                where T : PCAHelper, new()
```

Example usage:
```CSharp
// initialize with the client id and scopes. One can optionally pass special redirect URI
// else it creates one with commonly used: $"msal{clientId}://auth"
PCAHelper.Init<PCAHelper>(B2CConstants.ClientID, B2CConstants.Scopes);
// additional customization of the builder
PCAHelper.Instance.PCABuilder.WithB2CAuthority(B2CConstants.AuthoritySignInSignUp);
```

## Obtain the token
This API is for obtaining the token. It attempts to acquire a token silently and if that fails, presents an interactive sign-in dialogue to the user. It provides options to do silent, interactive and has the ability to customize each parameter builder.
One can also choose the preferred account.

``` CSharp
public async Task<AuthenticationResult> EnsureAuthenticatedAsync(
                                            bool doSilent = true,
                                            bool doInteractive = true,
                                            Func<IEnumerable<IAccount>, IAccount> preferredAccount = null,
                                            Action<AcquireTokenSilentParameterBuilder> customizeSilent = null,
                                            Action<AcquireTokenInteractiveParameterBuilder> customizeInteractive = null)
```

Example usage (B2C sample):
``` CSharp
var authResult = await PCAHelper.Instance.EnsureAuthenticatedAsync(
                                            doSilent:false,
                                            preferredAccount: (accounts) => GetAccountByPolicy(accounts, B2CConstants.PolicyEditProfile),
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