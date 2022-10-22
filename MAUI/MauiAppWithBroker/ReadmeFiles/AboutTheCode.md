## About the code

The structure of the solution is straightforward. All the application logic and UX reside in `MSALClient` folder.

- MSAL's main primitive for native clients, `PublicClientApplication`, is initialized as a static variable in `PCAWrapper.cs` (For details see [Client applications in MSAL.NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Client-Applications))

```CSharp
// Create PublicClientApplication once. Make sure that all the config parameters below are passed
PCA = PublicClientApplicationBuilder
                            .Create(AppConstants.ClientId)
                            .WithTenantId(AppConstants.TenantId)
                            .WithExperimentalFeatures() // this is for upcoming logger
                            .WithLogging(_logger)
                            .WithBroker()
                            .WithRedirectUri(PlatformConfig.Instance.RedirectUri)
                            .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                            .Build();
```

- For single-tenant apps, you must include `.WithTenantId(<tenantId>)` in the application builder.

- When the app tries to get an access token to make an API call after the sign in button is clicked (`MainPage.xaml.cs`) it will attempt to get a token without showing any UX - just in case a suitable token is already present in the cache from previous sessions. This is the code performing that logic:

```CSharp
try
{
    // First attempt silent login, which checks the cache for an existing valid token.
    // If this is very first time or user has signed out, it will throw MsalUiRequiredException
    AuthenticationResult result = await PCAWrapper.Instance.AcquireTokenSilentAsync(AppConstants.Scopes).ConfigureAwait(false);

    // call Web API to get the data
    string data = await CallWebAPIWithToken(result).ConfigureAwait(false);

    // show the data
    await ShowMessage("AcquireTokenTokenSilent call", data).ConfigureAwait(false);
}
catch (MsalUiRequiredException)
{
    // This executes UI interaction to obtain token
    AuthenticationResult result = await PCAWrapper.Instance.AcquireTokenInteractiveAsync(AppConstants.Scopes).ConfigureAwait(false);
    // call Web API to get the data
    string data = await CallWebAPIWithToken(result).ConfigureAwait(false);

    // show the data
    await ShowMessage("AcquireTokenInteractive call", data).ConfigureAwait(false);
}
catch (Exception ex)
{
    await ShowMessage("Exception in AcquireTokenTokenSilent", ex.Message).ConfigureAwait(false);
}
```

- If the attempt to obtain a token silently fails, we display a screen with the sign in button (at the bottom of the application).
- When the sign in button is pressed, we execute the same logic - but using a method that shows interactive UX:

  ```CSharp
  AuthenticationResult result = await PCA.AcquireTokenInteractive(App.Scopes, App.ParentWindow);
  ```

- The `Scopes` parameter indicates the permissions the application needs to gain access to the data requested through subsequent web API call (in this sample, encapsulated in `RefreshUserData`).

The `parentWindow` is used in Android to tie the authentication flow to the current activity, and in UWP to center the window. It is ignored on iOS. For more platform specific considerations, please see below.

- The sign out logic is very simple. In this sample we have just one user, however we are demonstrating a more generic sign out logic that you can apply if you have multiple concurrent users and you want to clear up the entire cache.

```CSharp
    var accounts = await App.PCA.GetAccountsAsync();
    while (accounts.Any())
    {
     await App.PCA.RemoveAsync(accounts.FirstOrDefault());
     accounts = await App.PCA.GetAccountsAsync();
    }
```

To enable the broker, you need to use the `WithBroker()` parameter when calling the `PublicClientApplicationBuilder.CreateApplication` method. `.WithBroker()` is set to true by default. Developers will also need to follow the steps below for [iOS](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Leveraging-the-broker-on-iOS#brokered-authentication-for-ios) or [Android](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Leveraging-the-broker-on-iOS#brokered-authentication-for-Android) applications.

### iOS specific considerations

The `Platforms\iOS` project only requires one extra line, in `AppDelegate.cs`.
You need to ensure that the OpenUrl handler looks as the snippet below:

```CSharp
public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
{
    if (AuthenticationContinuationHelper.IsBrokerResponse(null))
    {
        // Done on different thread to allow return in no time.
        _ = Task.Factory.StartNew(() => AuthenticationContinuationHelper.SetBrokerContinuationEventArgs(url));

        return true;
    }

    else if (!AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url))
    {
        return false;
    }

    return true;
}
```

Once again, this logic is meant to ensure that once the interactive portion of the authentication flow is concluded by the Authenticator app, the flow goes back to MSAL.

Also, in order to make the token cache work and have the `AcquireTokenSilentAsync` work multiple steps must be followed :

1. Enable Keychain access in your `Entitlements.plist` file and specify in the **Keychain Groups** your bundle identifier.
1. In your project options, on iOS **Bundle Signing view**, select your `Entitlements.plist` file for the Custom Entitlements field.
1. When signing a certificate, make sure XCode uses the same Apple Id.