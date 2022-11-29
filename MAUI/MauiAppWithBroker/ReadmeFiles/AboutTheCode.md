#### Using the Windows Web Account Manager (WAM)

This application is be able to authenticate users with the **Windows Web Account Manager** on machines using Windows 10 and above and does so by default. More information can be found [here](https://learn.microsoft.com/azure/active-directory/develop/scenario-desktop-acquire-token-wam)

#### Using the Microsoft Authenticator app on iOS

This application is be able to authenticate users with the **Microsoft Authenticator** for **iOS** which can be downloaded from the [App Store](https://apps.apple.com/ca/app/microsoft-authenticator/id983156458). When the user clicks sign in button they will be redirected to the **Microsoft Authenticator** application to provide their credentials. After this is successful they will be redirected to the main application.

#### Using the Microsoft Authenticator app on Android

This application is be able to authenticate users with the **Microsoft Authenticator** for **Android** which can be downloaded from the [Google Play Store](https://play.google.com/store/apps/details?id=com.azure.authenticator&gl=US). When the user clicks sign in button they will be redirected to the **Microsoft Authenticator** application to provide their credentials. After this is successful they will be redirected to the main application.

## About the code

The structure of the solution is straightforward. All the application logic and UX reside in `MSALClient` folder.

- MSAL's main primitive for native clients, `PublicClientApplication`, is initialized as a static variable in `MSALClientHelper.cs` (For details see [Client applications in MSAL.NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Client-Applications))

- When the app tries to get an access token to make an API call after the sign in button is clicked (`MainPage.xaml.cs`) it will attempt to get a token without showing any UX - just in case a suitable token is already present in the cache from previous sessions. This is the code performing that logic:

```CSharp
private async void OnSignInClicked(object sender, EventArgs e)
{
    await PublicClientSingleton.Instance.AcquireTokenSilentAsync();
    await Shell.Current.GoToAsync("scopeview");
}
```

- If the attempt to obtain a token silently fails, we display a screen with the sign in button (at the bottom of the application).
- When the sign in button is pressed, we execute the same logic - but using a method that shows interactive UX from the broker:

```CSharp
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
```

- The `Scopes` parameter indicates the permissions the application needs to gain access to the data requested through subsequent web API call.

The `parentWindow` is used in Android to tie the authentication flow to the current activity, and in WinUI to center the window. It is ignored on iOS. For more platform specific considerations, please see below.

- The sign out logic is very simple. In this sample we have just one user, however we are demonstrating a more generic sign out logic that you can apply if you have multiple concurrent users and you want to clear up the entire cache.

```CSharp
await this.PublicClientApplication.RemoveAsync(user).ConfigureAwait(false);
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

#### Configure application to handle the authentication callback

When MSAL.NET calls the broker on iOS, the broker will, in turn, call back to your application through the `OpenUrl` method of the `AppDelegate` class. Since MSAL will wait for the response from the broker, your application needs to cooperate to call MSAL.NET back. You do this by updating the `AppDelegate.cs` file to override the below method. This step is already completed in the `Platforms\iOS\AppDelegate.cs` file.

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

Similar to iOS, Android needs a way of ensuring that it is able to capture the authentication result from the interactive activity. This is done by overriding the `OnActivityResult` method in `MainActivity.cs`. (This step is already completed in `MainActivity.cs`.

```csharp
protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
{
  base.OnActivityResult(requestCode, resultCode, data);
  AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
}
```

This method is invoked every time the application is launched and is used as an opportunity to process the response from the broker and complete the authentication process initiated by MSAL.NET.

#### [iOS only] Register a URL

MSAL.NET uses URLs to invoke the broker and then return the broker response back to your app. To finish the round trip, you need to register a URL scheme for your app in the `Info.plist` file.

The `CFBundleURLSchemes` name must include `msauth.` as a prefix, followed by your `CFBundleURLName`.

`$"msauth(BundleId)"`

**For example:**
`msauth.com.companyname.mauiappwithbroker`

**Note** This will become part of the RedirectUri used for uniquely identifying your app when receiving the response from the broker.

```CSharp
 <key>CFBundleURLTypes</key>
    <array>
      <dict>
        <key>CFBundleTypeRole</key>
        <string>Editor</string>
        <key>CFBundleURLName</key>
        <string>com.companyname.mauiappwithbroker</string>
        <key>CFBundleURLSchemes</key>
        <array>
            <!-- Replace bff27aee-5b7f-4588-821a-ed4ce373d8e2 with your application's application ID-->
            <string>msalbff27aee-5b7f-4588-821a-ed4ce373d8e2</string>
            <string>msauth.com.companyname.mauiappwithbroker</string>
        </array>
      </dict>
    </array>
```

#### [iOS only] LSApplicationQueriesSchemes

MSAL uses `canOpenURL:` to check if the broker is installed on the device. In iOS 9, Apple locked down what schemes an application can query for.

**Add** **`msauthv2`** to the `LSApplicationQueriesSchemes` section of the `Info.plist` file.

```CSharp
<key>LSApplicationQueriesSchemes</key>
    <array>
      <string>msauthv2</string>
      <string>msauthv3</string>
    </array>
```

#### [Android only, optional] System Browser configuration with Android Broker redirect URI

If you are using the system browser for interactive authentication, it is possible you will have configured your application to use brokered authentication when the device does not have broker installed. In this scenario, MSAL will try to authenticate using the default system browser in the device. This will fail out of the box because the redirect URI is configured for broker and the system browser would know how to use it to navigate back to MSAL. To resolve this, you can configure what is known as an intent filter with the broker redirect URI that you used in step four. You will need to modify your application's manifest to add the intent filter as shown below.

```xml
//NOTE: the slash before your signature value added to the path attribute
//This uses the base64 encoded signature produced above.
      <intent-filter>
        <action android:name="android.intent.action.VIEW" />
        <category android:name="android.intent.category.DEFAULT" />
        <category android:name="android.intent.category.BROWSABLE" />
        <data android:scheme="msal"Client ID"" android:host="auth" />
        <data android:scheme="msauth"
                    android:host="package name"
                    android:path="/Package Signature"/>
      </intent-filter>
```

for example, if you have a redirect URI of `msauth://com.microsoft.xforms.testApp/hgbUYHVBYUTvuvT&Y6tr554365466=` and a client id of `76hdu2l6-df67-49d0-2d0b-cd95kjny6592` your manifest should look something like

```xml
//NOTE: the slash before your signature value added to the path attribute
//This uses the base64 encoded signature produced above.
      <intent-filter>
        <action android:name="android.intent.action.VIEW" />
        <category android:name="android.intent.category.DEFAULT" />
        <category android:name="android.intent.category.BROWSABLE" />
        <data android:scheme="msal4a1aa1d5-c567-49d0-ad0b-cd957a47f842" android:host="auth" />
        <data android:scheme="msauth"
                    android:host="com.companyname.XamarinDev"
                    android:path="/hgbUYHVBYUTvuvT&Y6tr554365466="/>
      </intent-filter>
```

>Please be sure to add a / in front of the signature in the "android:path" value
