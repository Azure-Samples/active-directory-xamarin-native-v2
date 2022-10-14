---
services: active-directory
platforms: dotnet, MAUI.iOS, MAUI.Android, Windows
author: v-michaelmi
level:  300
client: MAUI 
service: Microsoft Graph 
endpoint: Microsoft identity platform
---
# Integrate Microsoft identity and the Microsoft Graph into a MAUI app using MSAL

[![Build status](https://identitydivision.visualstudio.com/IDDP/_apis/build/status/AAD%20Samples/.NET%20client%20samples/CI%20of%20Azure-Samples%20--%20xamarin-native-v2)](https://identitydivision.visualstudio.com/IDDP/_build/latest?definitionId=32)

## About this sample

This is a simple [Multi-platform App UI (MAUI)](https://dotnet.microsoft.com/en-us/apps/maui) app showcasing how to use MSAL.NET to:

1. Authenticate users with Work or School accounts (AAD) or Microsoft personal accounts (MSA)
2. Get an access token for Microsoft Graph

The MAUI application is provided for MAUI.iOS, MAUI.Android, and MAUI.WinUI

![Topology](./ReadmeFiles/Topology.png)

- You can learn more about Microsoft Graph with this video: [An introduction to Microsoft Graph for developers](https://www.youtube.com/watch?v=EBbnpFdB92A).

## How To Run this Sample

To run this sample you will need:

- [Visual Studios](https://aka.ms/vsdownload) with the **MAUI** workload:
    * [Instructions for Windows](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation?tabs=vswin)
    * [Instructions for MacOS](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation?tabs=vsmac)
- An Internet connection
- At least one of the following accounts:
  - A Microsoft Account - you can get a free account by visiting [https://www.microsoft.com/en-us/outlook-com/](https://www.microsoft.com/en-us/outlook-com/).
  - An Azure AD account - you can get a free trial Office 365 account by visiting [https://products.office.com/en-us/try](https://products.office.com/en-us/try).

### Step 1:  Clone or download this repository

From your shell or command line:

```Shell
git clone https://github.com/Azure-Samples/active-directory-xamarin-native-v2.git
cd MAUI/MauiAppBasic
```

or download and exact the repository .zip file.

> Given that the name of the sample is pretty long, and so are the name of the referenced NuGet packages, you might want to clone it in a folder close to the root of your hard drive, to avoid file size limitations on Windows.

### [OPTIONAL] Step 2:  Register the sample on the app registration portal

You can run the sample as is with its current settings, or you can optionally register it as a new application under your own developer account.

#### Choose the Azure AD tenant where you want to create your applications

As a first step you'll need to:

1. Sign in to the [Azure portal](https://portal.azure.com) using either a work or school account or a personal Microsoft account.
1. If your account is present in more than one Azure AD tenant, select your profile at the top right corner in the menu on top of the page, and then **switch directory**.
   Change your portal session to the desired Azure AD tenant.

#### Register the client app (active-directory-xamarin-native-v2)

1. Navigate to the Microsoft identity platform for developers [App registrations](https://go.microsoft.com/fwlink/?linkid=2083908) page.
1. Select **New registration**.
1. When the **Register an application page** appears, enter your application's registration information:
   - In the **Name** section, enter a meaningful application name that will be displayed to users of the app, for example `active-directory-xamarin-native-v2`.
   - In the **Supported account types** section, select **Accounts in any organizational directory and personal Microsoft accounts (e.g. Skype, Xbox, Outlook.com)**.
1. Select **Register** to create the application.
1. On the app **Overview** page, find the **Application (client) ID** value and record it for later. You'll need it to configure the Visual Studio configuration file for this project.
1. In the list of pages for the app, select **Authentication**..
   - In the **Redirect URIs** | **Suggested Redirect URIs for public clients (mobile, desktop)** section, check **the option of the form msal&lt;clientId&gt;://auth**
      * If you want to allow for broswer redirect while using the Windows Application be sure to add 'http://localhost' as a redirect URI as well
1. Select **Save**.
1. In the list of pages for the app, select **API permissions**
   - Click the **Add a permission** button and then,
   - Ensure that the **Microsoft APIs** tab is selected
   - In the *Commonly used Microsoft APIs* section, click on **Microsoft Graph**
   - In the **Delegated permissions** section, ensure that the right permissions are checked: **User.Read**. Use the search box if necessary.
   - Select the **Add permissions** button

### [OPTIONAL] Step 3: Configure the Visual Studio project with your app coordinates

In the steps below, "ClientID" is the same as "Application ID" or "AppId".

1. Open the solution in Visual Studio.
2. Open the `MSALClient\AppConstants.cs` file.
3. Find the assignment for `internal const string ClientId` and replace the value with the Application ID from the app registration portal, created in Step 2.

#### [OPTIONAL] Step 3a: Configure the iOS project with your apps' return URI

1. Open the `Platforms/iOS/AppDelegate.cs` file.
1. replace the `iOSRedirectURI` with the redirect URI of your application:

```CSharp
private const string iOSRedirectURI = "msauth.com.companyname.mauiappbasic://auth"; // TODO - Replace with your redirectURI
```

where `[ClientID]` is the identifier you copied in step 2. Save the file.

#### [OPTIONAL] Step 3b: Configure the Android project with your return URI

1. Open the `Platforms\Android\MsalActivity.cs` file.
1. Replace `[ClientID]` with the identifier you copied in step 2.
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

### Step 4:  Run the sample

Choose the platform you want to work on by setting the startup project in the Solution Explorer. Make sure that your platform of choice is marked for build and deploy in the Configuration Manager.
Clean the solution, rebuild the solution, and run it:

- Click the sign-in button at the bottom of the application screen. On the sign-in screen, enter the name and password of a personal Microsoft account or a work/school account. The sample works exactly in the same way regardless of the account type you choose, apart from some visual differences in the authentication and consent experience. During the sign in process, you will be prompted to grant various permissions (to allow the application to access your data).
- Upon successful sign in and consent, the application screen will list some basic profile info for the authenticated user. Also, the button at the bottom of the screen will turn into a Sign out button.
- Close the application and reopen it. You will see that the app retains access to the API and retrieves the user info right away, without the need to sign in again.
- Sign out by clicking the Sign out button and confirm that you lose access to the API until the next interactive sign in.

#### Running in an Android Emulator

MSAL.NET in Android requires support for Chrome Custom Tabs for displaying authentication prompts.
Not every emulator image comes with Chrome on board: please refer to [this document](https://github.com/Azure-Samples/active-directory-general-docs/blob/master/AndroidEmulator.md) for instructions on how to ensure that your emulator supports the features required by MSAL.

## About the code

The structure of the solution is straightforward. All the application logic and UX reside in ``UserDetailsClient (portable)`` project.

- MSAL's main primitive for native clients, `PublicClientApplication`, is initialized as a static variable in `MSALClient\PCAWrapper.cs` (For details see [Client applications in MSAL.NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Client-Applications))

  ```CSharp
// Create PCA once. Make sure that all the config parameters below are passed
PCA = PublicClientApplicationBuilder
                            .Create(AppConstants.ClientId)
                            .WithExperimentalFeatures() // this is for upcoming logger
                            .WithLogging(_logger)
                            .WithRedirectUri(PlatformConfig.Instance.RedirectUri)
                            .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                            .Build();
  ```

- For single-tenant apps, you must include `.WithTenantId(<tenantId>)` in the application builder.

- At application startup, the main page (`MainPage.xaml.cs`) attempts to get a token without showing any UX - just in case a suitable token is already present in the cache from previous sessions. This is the code performing that logic:

  ```CSharp
try
{
    PCAWrapper.Instance.UseEmbedded = this.useEmbedded.IsChecked;
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

- If the attempt to obtain a token silently fails a sign-in screen will appear.
    * Using the 'Embedded' login screen will prompt your user to login from a desktop screen
    * Default login behavior will use the devices embedded browser to prompt a login with the authorization host. By default this application uses 'login.microsoft.com'    
- When the sign in button is pressed, we execute the same logic - but using a method that shows interactive UX:

  ```CSharp
// This executes UI interaction to obtain token
AuthenticationResult result = await PCAWrapper.Instance.AcquireTokenInteractiveAsync(AppConstants.Scopes).ConfigureAwait(false);
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

## Troubleshooting

### Some projects don't load in Visual Studio

This might be because you have not installed all the required components from Visual Studio. You need to add the **.NET Mutli-platform App UI development** [workload](https://learn.microsoft.com/en-us/visualstudio/install/modify-visual-studio?view=vs-2022), in the Visual Studio Installer.

### The project you want is not built

you need to right click on the visual studio solution, choose **Configuration Properties** > **Configuration** and make sure that you check the projects and configuration you want to build (and deploy)

## Moving from sample to production

Samples favor simple code structures that show you how to use MSAL. Samples do not showcase best practices and patterns, nor do they make use of other libraries.

- Consider using [dependency injection](https://medium.com/syncfusion/learn-how-to-use-dependency-injection-in-net-maui-800f1bf9bc4d) for the `IPublicClientApplication` 
- Consider wrapping the construction of the `IPublicClientApplication` and `AcquireToken*` in another class to make testing possible. Mocking the existing builder pattern for creating `IPublicClientApplication` and `AcquireTokenInteractiveParameterBuilder` is not possible (we've tried).
- MSAL will generally let HTTP exceptions propagate. Consider using [Maui.Networking.Connectivity](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/communication/networking?tabs=windows) to detect situations where the network is down in order to provide a better error message to the user. 

## More information

For more information, please visit:

- For more information on acquiring tokens with MSAL.NET, please visit [MSAL.NET's conceptual documentation](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki), in particular:
  - [PublicClientApplication](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Client-Applications#publicclientapplication)
  - [Recommended call pattern in public client applications](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/AcquireTokenSilentAsync-using-a-cached-token#recommended-call-pattern-in-public-client-applications)
  - [Acquiring tokens interactively in public client application flows](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Acquiring-tokens-interactively)
- To understand more about the Microsoft identity platform endpoint see http://aka.ms/aaddevv2
- For more information about how the protocols work in this scenario and other scenarios, see [Authentication Scenarios for Microsoft identity platform](http://go.microsoft.com/fwlink/?LinkId=394414).
- For more information about Microsoft Graph, please visit [the Microsoft Graph homepage](https://graph.microsoft.io/en-us/)
