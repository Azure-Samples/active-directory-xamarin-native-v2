## About the code

The structure of the solution is straightforward. All the application logic and UX reside in ``UserDetailsClient (portable)`` project.

- MSAL's main primitive for native clients, `PublicClientApplication`, is initialized as a static variable in `MauiAppWithBroker\MSALClient\MSALClientHelper.cs` (For details see [Client applications in MSAL.NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Client-Applications))

- For single-tenant apps, you must include `.WithTenantId(<tenantId>)` in the application builder.

- At application startup, the main page (`MainPage.xaml.cs`) attempts to get a token without showing any UX - just in case a suitable token is already present in the cache from previous sessions. This is the code performing that logic:

```CSharp
try
private async void OnSignInClicked(object sender, EventArgs e)
{
    // Sign-in the user
    PublicClientSingleton.Instance.UseEmbedded = this.useEmbedded.IsChecked;
    await PublicClientSingleton.Instance.AcquireTokenSilentAsync();

    await Shell.Current.GoToAsync("userview");
}
```

- If the attempt to obtain a token silently fails a sign-in screen will appear.
    * Using the 'Embedded' login screen will prompt your user to login from a desktop screen
    * Default login behavior will use the devices embedded browser to prompt a login with the authorization host. By default this application uses 'login.microsoft.com'
- When the sign in button is pressed, we execute the same logic - but using a method that shows interactive UX:

```CSharp
            if (this.PublicClientApplication.IsUserInteractive())
            {
                if (UseEmbedded)
                {
                    return await this.PublicClientApplication.AcquireTokenInteractive(scopes)
                        .WithLoginHint(existingAccount?.Username ?? String.Empty)
                        .WithUseEmbeddedWebView(true)
                        .WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
                        .ExecuteAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    SystemWebViewOptions systemWebViewOptions = new SystemWebViewOptions();
#if IOS
                    // Hide the privacy prompt in iOS
                    systemWebViewOptions.iOSHidePrivacyPrompt = true;
#endif
                    return await this.PublicClientApplication.AcquireTokenInteractive(scopes)
                        .WithLoginHint(existingAccount?.Username ?? String.Empty)
                        .WithSystemWebViewOptions(systemWebViewOptions)
                        .WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
                        .ExecuteAsync()
                        .ConfigureAwait(false);
                }
            }
  ```

- The `Scopes` parameter indicates the permissions the application needs to gain access to the data requested through subsequent web API call.

The `parentWindow` is used in Android to tie the authentication flow to the current activity, and in UWP to center the window. It is ignored on iOS. For more platform specific considerations, please see below.

### Platform specific considerations

The platform specific projects require only a couple of extra lines to accommodate for individual platform differences.

### Android specific considerations

The `Platforms\Android` project requires two extra lines in the `MainActivity.cs` file.
In `OnActivityResult`, we need to add

```CSharp
AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
```

That line ensures that the control goes back to MSAL once the interactive portion of the authentication flow ended.

In `OnCreate`, we need to add the following assignment (the ParentWindow is the activity)

```CSharp
App.ParentWindow = this;
```

That code ensures that the authentication flows occur in the context of the current activity.

### iOS specific considerations

The `Platforms\iOS` project only requires one extra line, in `AppDelegate.cs`.
You need to ensure that the iOSRedirectURI is set to the proper redirect URI for your iOS app as shown below.

```CSharp
private const string iOSRedirectURI = "msauth.com.companyname.mauiappbasic://auth"; // TODO - Replace with your redirectURI
```

Once again, this logic is meant to ensure that once the interactive portion of the authentication flow is concluded, the flow goes back to MSAL.

Also, in order to make the token cache work and have the `AcquireTokenSilentAsync` work multiple steps must be followed :

1. Enable Keychain access in your `Entitlements.plist` file and specify in the **Keychain Groups** your bundle identifier.
1. In your project options, on iOS **Bundle Signing view**, select your `Entitlements.plist` file for the Custom Entitlements field.
1. When signing a certificate, make sure XCode uses the same Apple Id.

#### Broker support

On Android and iOS, brokers enable:

- Single Sign On (SSO). Your users won't need to sign-in to each application.
- Device identification. By accessing the device certificate which was created on the device when it was workplace joined.
- Application identification verification. When an application calls the broker, it passes its redirect url, and the broker verifies it.

You can learn how to have your application support the broker on iOS in [Leveraging the broker on iOS](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Leveraging-the-broker-on-iOS)
