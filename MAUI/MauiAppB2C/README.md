---
services: active-directory
platforms: dotnet, MAUI.iOS, MAUI.Android, Windows
author: v-michaelmi
level:  300
client: MAUI 
service: Microsoft Graph 
endpoint: Microsoft identity platform
---
# Integrate Microsoft identity, the Microsoft Authenticator B2C App (broker), and Microsoft Graph into a MAUI app using MSAL

[![Build status](https://identitydivision.visualstudio.com/IDDP/_apis/build/status/AAD%20Samples/.NET%20client%20samples/CI%20of%20Azure-Samples%20--%20xamarin-native-v2)](https://identitydivision.visualstudio.com/IDDP/_build/latest?definitionId=32)

## About this sample

This is a simple [Multi-platform App UI (MAUI)](https://dotnet.microsoft.com/en-us/apps/maui) app showcasing how to use MSAL.NET to:

1. Authenticate users with Work or School accounts (AAD) or Microsoft personal accounts (MSA) using B2C
2. Get an access token for Microsoft Graph

The MAUI application is provided for MAUI.iOS, MAUI.Android, and MAUI.WinUI


![Topology](./ReadmeFiles/Topology.png)

- You can learn more about Microsoft Graph with this video: [An introduction to Microsoft Graph for developers](https://www.youtube.com/watch?v=EBbnpFdB92A).

## What is the broker?

On Android and iOS, brokers enable:

- Single Sign On (SSO) - your users won't need to sign-in to each application.
- Device identification - by accessing the device certificate which was created on the device when it was workplace joined.
- Application identification verification - when an application calls the broker, it passes its redirect url, and the broker verifies it.

## How To Run this Sample

To run this sample you will need:

- [Visual Studios](https://aka.ms/vsdownload) with the **MAUI** workload:
    * [Instructions for Windows](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation?tabs=vswin)
    * [Instructions for MacOS](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation?tabs=vsmac)
- An Internet connection
- At least one of the following accounts:
  - A Microsoft Account - you can get a free account by visiting [https://www.microsoft.com/en-us/outlook-com/](https://www.microsoft.com/en-us/outlook-com/).
  - An Azure AD account - you can get a free trial Office 365 account by visiting [https://products.office.com/en-us/try](https://products.office.com/en-us/try).
- [An Azure Active Directory B2C Tenant](https://learn.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant).

### Step 1:  Clone or download this repository

From your shell or command line:

```Shell
git clone https://github.com/Azure-Samples/active-directory-xamarin-native-v2.git
cd 2-With-broker
```

or download and exact the repository .zip file.

> Given that the name of the sample is pretty long, and so are the name of the referenced NuGet packages, you might want to clone it in a folder close to the root of your hard drive, to avoid file size limitations on Windows.

### Step 2:  Register the sample on the app registration portal

These steps are identical to the ones in 1-Basic, so if you've already set-up an app for use there, feel free to reuse it for this example.

#### Choose the Azure AD tenant where you want to create your applications

As a first step you'll need to:

1. Sign in to the [Azure portal](https://portal.azure.com) using either a work or school account or a personal Microsoft account.
1. If your account is present in more than one Azure AD tenant, select your profile at the top right corner in the menu on top of the page, and then **switch directory**.
   Change your portal session to the desired Azure AD tenant.

#### Register the client app (active-directory-xamarin-native-v2)

1. [Configure authentication in a sample web application using Azure Active Directory B2C](https://docs.microsoft.com/azure/active-directory-b2c/configure-authentication-sample-web-app)
1. Navigate to your Azure B2C Tenant
1. Navigate to the Microsoft identity platform for developers [App registrations](https://go.microsoft.com/fwlink/?linkid=2083908) page.
1. Select **New registration**.
1. When the **Register an application page** appears, enter your application's registration information:
   - In the **Name** section, enter a meaningful application name that will be displayed to users of the app, for example `active-directory-xamarin-native-v2`.
   - In the **Supported account types** section, select **Accounts in any organizational directory and personal Microsoft accounts (e.g. Skype, Xbox, Outlook.com)**.
1. Select **Register** to create the application.
1. On the app **Overview** page, find the **Application (client) ID** value and record it for later. You'll need it to configure the Visual Studio configuration file for this project.
1. In the list of pages for the app, select **Authentication**.
   - In the **Redirect URIs** | **Suggested Redirect URIs for public clients (mobile, desktop)** section, check **the option of the form msal&lt;clientId&gt;://auth**

1. Select **Save**.
1. In the list of pages for the app, select **API permissions**
   - Click the **Add a permission** button and then,
   - Check to see if your app has Microsoft Graph's User.Read permissions. If so, skip to step 3.
   - Otherwise, Ensure that the **Microsoft APIs** tab is selected
   - In the *Commonly used Microsoft APIs* section, click on **Microsoft Graph**
   - In the **Delegated permissions** section, ensure that the right permissions are checked: **User.Read**. Use the search box if necessary.
   - Select the **Add permissions** button

### Step 3:  Configure the Visual Studio project with your app coordinates

In the steps below, "ClientID" is the same as "Application ID" or "AppId". 
1. Open the solution in Visual Studio.
1. Open the `MSALClient\B2CConstants.cs` file.
1. Replace the following values as instructed:
    * Set `Tenant` to be the domain of your tenant.
    * Set `AzureADB2CHostname` to be the domain of your login. This should usually be the first part of your tenant domain followed by `b2clogin.com`. E.g. `mytenant.b2clogin.com` 
    * Set `ClientID` to be the same as the `Application ID`
    * Set `PolicySignUpSignIn` to be the sign in/sign out policy used on your tenant

#### [OPTIONAL] Step 3a: Configure the iOS project with your apps' return URI

1. Open the `Platforms/iOS/AppDelegate.cs` file.
1. replace the `iOSRedirectURI` with the redirect URI of your application:

```CSharp
private const string iOSRedirectURI = "msauth.com.companyname.mauiappbasic://auth"; // TODO - Replace with your redirectURI
```

where `[ClientID]` is the identifier you copied in step 2. Save the file.

#### [Android specific] Step 3b: Configure the Android project with your return URI

1. Open the `Platforms\Android\MsalActivity.cs` file.
1. Replace `[ClientID]` as noted in the example below with the identifier you copied in step 2.
1. Save the file.
```csharp
  [Activity]
  [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
        DataHost = "auth",
        DataScheme = "msal[ClientID]")]
  public class MsalActivity : BrowserTabActivity
  {
  }
```

### Step 4: Set up integration with the broker (Microsoft Authenticator app)

Steps 4a and 4b are explanation only, steps 4c-4e are iOS only. Both will require step 4f, so you can skip straight there if you're working on configuring Android.

There are different broker apps, based on platform:
* iOS and Android can use the [Microsoft Authenticator](https://itunes.apple.com/us/app/microsoft-authenticator/id983156458) app
* Android can use ([Intune Company Portal](https://play.google.com/store/apps/details?id=com.microsoft.windowsintune.companyportal&hl=en_US) as a broker)

#### Step 4a: Enable broker support
Broker support is enabled on a per-PublicClientApplication basis. It is disabled by default. You must use the `WithBroker()` parameter (set to true by default) when creating the PublicClientApplication through the PublicClientApplicationBuilder.

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

#### Step 4b: Configure application to handle the authentication callback
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

Similar to iOS, Android needs a way of ensuring that it is able to capture the authentication result from the interactive activity. This is done by overriding the `OnActivityResult` method in `MainActivity.cs`. (This step is already completed [in MainActivity.cs](https://github.com/Azure-Samples/active-directory-xamarin-native-v2/blob/3d877ba0ded3e644610654f6ad1ca7abb30f9e5b/2-With-broker/UserDetailsClient/UserDetailsClient.Droid/MainActivity.cs#L27))

```csharp
protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
{
  base.OnActivityResult(requestCode, resultCode, data);
  AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
}
```

This method is invoked every time the application is launched and is used as an opportunity to process the response from the broker and complete the authentication process initiated by MSAL.NET.

#### [iOS only] Step 4d: Register a URL Scheme
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

#### [iOS only] Step 4e: LSApplicationQueriesSchemes
MSAL uses `canOpenURL:` to check if the broker is installed on the device. In iOS 9, Apple locked down what schemes an application can query for. 

**Add** **`msauthv2`** to the `LSApplicationQueriesSchemes` section of the `Info.plist` file.

```CSharp 
<key>LSApplicationQueriesSchemes</key>
    <array>
      <string>msauthv2</string>
      <string>msauthv3</string>
    </array>
```

#### Step 4f: Register your RedirectUri in the application portal
Using the broker adds an extra requirement on your redirectUri. The redirectUri _**must**_ have the following format:

**For iOS:**
```CSharp
$"msauth.{BundleId}://auth"
```
**Example for iOS:**
```CSharp
public static string redirectUriOnIos = "msauth.com.yourcompany.MAUI://auth"; 
```
**You'll notice the iOS RedirectUri matches the `CFBundleURLSchemes` name you included in the `Info.plist` file.**

**For Android:**
```CSharp
$"msauth://{Package Name}/{Signature Hash}"
```
**Example for Android:**
```CSharp
public static string redirectUriOnIAndroid = "msauth://UserDetailsClient.Droid/hgbUYHVBYUTvuvT&Y6tr554365466="; 
```

Once you have created your redirect URI, you can register it by navigating to the **Authentication** tab.

- **Android:** Select **Add a platform** under the **Platform Configurations** section, click on **Android** and add your package name and the signature hash.

- **iOS:** Select **Add a platform** under the **Platform Configurations** section, click on **iOS** and add your iOS bundle id.

Once you have the proper redirect URI, update the `BrokerRedirectUriOnIos` and the `BrokerRedirectUriOnAndroid` fields in the App.cs file with the values.

#### Android Specific Redirect URI Configuration

The redirect uri on Android will need to be created based on the signature of the .APK used to sign it. This means that it will be different depending on where this sample is run because Visual Studio creates a unique signing key for debugging purposes on every machine. You can figure out what that signature will be by running the following commands

- For Windows: `keytool -exportcert -alias androiddebugkey -keystore %HOMEPATH%\.android\debug.keystore | openssl sha1 -binary | openssl base64`
- For Mac: `keytool -exportcert -alias androiddebugkey -keystore ~/.android/debug.keystore | openssl sha1 -binary | openssl base64`

Keytool is part of the standard Java distribution. You may need to the add the jdk to your path, and you can normally find the jdk in `C:\Program Files\Java\jdk1.8.0_251\bin`. Keytool is typically used for managing keys and certifications you can sign things with with. More information on the [keytool here](https://docs.oracle.com/cd/E82085_01/140/rib_security_guide/appendixB.htm). Keytool also relies on OpenSSL, so you may also need to [install it from this website](https://www.openssl.org/).

Once you have your signature, simply use the `msauth://{Package Name}/{Signature Hash}` format as shown above to create your redirect URI.

NOTE: if you are still having trouble calculating the correct redirect URI for the broker, MSAL will thrown an exception with the correct broker redirect URI in the error message. if you see this exception, simply update your redirect URI in the code and portal with the one acquired fromt the exception as shown above.

NOTE: You also have the option of acquiring your redirect URI with code. see [Brokered Authentication for Android](https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-net-use-brokers-with-xamarin-apps) for more details

#### [Android only, optional] Step 4g: System Browser configuration with Android Broker redirect URI

If you are using the system browser for interactive authentication, it is possible you will have configured your application to use brokered authentication when the device does not have broker installed. In this scenario, MSAL will try to authenticate using the default system browser in the device. This will fail out of the box because the redirect URI is configured for broker and the system browser would know how to use it to navigate back to MSAL. To resolve this, you can configure what is known as an intent filter with the broker redirect URI that you used in step four. You will need to modify your application's manifest to add the intent filter as shown below.

```
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

```
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
**Please be sure to add a / in front of the signature in the "android:path" value**

### Step 5: Run the sample!

Make sure the platform you configured is the same one you mark for build and deployment.

Clean the solution, build and run it:

- Click the sign-in with broker button at the bottom of the application screen. Depending on whether or not there are conditional access policies applied to the user signing in, there will be two different experiences. If there are conditional access policies applied to the user signing in:
    - The Authenticator App will open, or ask you to install it from the App Store. 
    - In the Authenticator App, either select an already existing account, or add a new one.
    - If adding a new one, on the sign-in screen, enter the name and password of a personal Microsoft account or a work/school account. The authenticator app works exactly in the same way regardless of the account type you choose, apart from some visual differences in the authentication and consent experience. During the sign in process, you will be prompted to grant various permissions (to allow the application to access your data).
    - Upon successful sign in and consent, the application screen will list some basic profile info for the authenticated user. Also, the button at the bottom of the screen will turn into a Sign out button.
    - Close the application and reopen it. You will see that the app retains access to the API and retrieves the user info right away, without the need to sign in again.
    - Sign out by clicking the Sign out button and confirm that you lose access to the API until the next interactive sign in.
- If there are no conditional access policies applied to the user signing in, signing in with a broker is not required, so you will see the typical sign-in UI flow.

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
    AuthenticationResult result = await PCAWrapperB2C.Instance.AcquireTokenSilentAsync(B2CConstants.Scopes).ConfigureAwait(false);

    string claims = GetClaims(result);

    // show the claims
    await ShowMessage("AcquireTokenTokenSilent call Claims", claims).ConfigureAwait(false);
}
catch (MsalUiRequiredException)
{
    // This executes UI interaction to obtain token
    AuthenticationResult result = await PCAWrapperB2C.Instance.AcquireTokenInteractiveAsync(B2CConstants.Scopes).ConfigureAwait(false);

    string claims = GetClaims(result);

    // show the Claims
    await ShowMessage("AcquireTokenInteractive call Claims", claims).ConfigureAwait(false);
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

## Troubleshooting

### Some projects don't load in Visual Studio

This might be because you have not installed all the required components from Visual Studio. You need to add the **.NET Mutli-platform App UI development** [workload](https://learn.microsoft.com/en-us/visualstudio/install/modify-visual-studio?view=vs-2022), in the Visual Studio Installer.

### The project you want is not built

you need to right click on the visual studio solution, choose **Configuration Properties** > **Configuration** and make sure that you check the projects and configuration you want to build (and deploy)

## More information

For more information, please visit:

- For more information on acquiring tokens with MSAL.NET, please visit [MSAL.NET's conceptual documentation](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki), in particular:
  - [iOS Broker](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Leveraging-the-broker-on-iOS)
  - [How to migrate ADAL broker apps to MSAL broker apps on iOS](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/How-to-migrate-from-using-iOS-Broker-on-ADAL.NET-to-MSAL.NET)
  - [PublicClientApplication](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Client-Applications#publicclientapplication)
  - [Recommended call pattern in public client applications](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/AcquireTokenSilentAsync-using-a-cached-token#recommended-call-pattern-in-public-client-applications)
  - [Acquiring tokens interactively in public client application flows](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Acquiring-tokens-interactively)
- To understand more about the AAD V2 endpoint see http://aka.ms/aaddevv2
- For more information about how the protocols work in this scenario and other scenarios, see [Authentication Scenarios for Azure AD](http://go.microsoft.com/fwlink/?LinkId=394414).
- For more information about Microsoft Graph, please visit [the Microsoft Graph homepage](https://graph.microsoft.io/en-us/)
