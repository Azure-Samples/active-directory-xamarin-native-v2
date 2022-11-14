---
page_type: sample
name: A .NET MAUI app using MSAL.NET to sign-in users and calling MS Graph Api
description: Integrate Microsoft identity, the Microsoft Authenticator B2C App (broker), and Microsoft Graph into a MAUI app using MSAL
languages:
 -  csharp
products:
 - maui
 - azure-active-directory-b2c
urlFragment: active-directory-xamarin-native-v2
extensions:
- services: ms-identity
- platform: MAUI
- endpoint: AAD v2.0
- level: 200
- client: MAUI (iOS, Android, UWP)
- service: Microsoft Graph
---

# Integrate Microsoft identity, the Microsoft Authenticator B2C App (broker), and Microsoft Graph into a MAUI app using MSAL

[![Build status](https://identitydivision.visualstudio.com/IDDP/_apis/build/status/AAD%20Samples/.NET%20client%20samples/CI%20of%20Azure-Samples%20--%20xamarin-native-v2)](https://identitydivision.visualstudio.com/IDDP/_build/latest?definitionId=32)

* [Overview](#overview)
* [Scenario](#scenario)
* [Prerequisites](#prerequisites)
* [Setup the sample](#setup-the-sample)
* [Explore the sample](#explore-the-sample)
* [Troubleshooting](#troubleshooting)
* [About the code](#about-the-code)
* [Next Steps](#next-steps)
* [Contributing](#contributing)
* [Learn More](#learn-more)

## Overview

This is a simple [Multi-platform App UI (MAUI)](https://dotnet.microsoft.com/en-us/apps/maui) app showcasing how to use MSAL.NET to authenticate users with Work or School accounts (AAD) or Microsoft personal accounts (MSA) using B2C

The MAUI application is provided for MAUI.iOS, MAUI.Android, and MAUI.WinUI

## Scenario

This sample demonstrates a MAUI (iOS, Android, UWP) calling Microsoft Graph.

1. The client MAUI (iOS, Android, UWP) uses the [MSAL.NET](https://aka.ms/msal-net) to sign-in a user and obtain a JWT [ID Token](https://aka.ms/id-tokens) and an [Access Token](https://aka.ms/access-tokens) from **Azure AD B2C**.
1. The **access token** is used as a *bearer* token to authorize the user to call the Microsoft Graph protected by **Azure AD B2C**.

![Topology](./ReadmeFiles/Topology.png)

## How To Run this Sample

To run this sample you will need:

- [Visual Studios](https://aka.ms/vsdownload) with the **MAUI** workload:
    * [Instructions for Windows](https://learn.microsoft.com/dotnet/maui/get-started/installation?tabs=vswin)
    * [Instructions for MacOS](https://learn.microsoft.com/dotnet/maui/get-started/installation?tabs=vsmac)
- An Internet connection
- At least one of the following accounts:
  - A Microsoft Account - you can get a free account by visiting [https://www.microsoft.com/en-us/outlook-com/](https://www.microsoft.com/en-us/outlook-com/).
  - An Azure AD account - you can get a free trial Office 365 account by visiting [https://products.office.com/en-us/try](https://products.office.com/en-us/try).
- [An Azure Active Directory B2C Tenant](https://learn.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant).

## Setup the sample

### Step 1:  Clone or download this repository

From your shell or command line:

```console
git clone https://github.com/Azure-Samples/active-directory-xamarin-native-v2.git
cd MAUI/MauiAppB2C
```

or download and extract the repository *.zip* file.

> Given that the name of the sample is pretty long, and so are the name of the referenced NuGet packages, you might want to clone it in a folder close to the root of your hard drive, to avoid file size limitations on Windows.

### Step 3: Register the sample application(s) in your tenant

> :warning: This sample comes with a pre-registered application for demo purposes. If you would like to use your own **Azure AD B2C** tenant and application, follow the steps below to register and configure the application on **Azure portal**. Otherwise, continue with the steps for [Running the sample](#running-the-sample).

- follow the steps below for manually register your apps

#### Choose the Azure AD B2C tenant where you want to create your applications

To manually register the apps, as a first step you'll need to:

1. Sign in to the [Azure portal](https://portal.azure.com).
1. If your account is present in more than one Azure AD B2C tenant, select your profile at the top right corner in the menu on top of the page, and then **switch directory** to change your portal session to the desired Azure AD B2C tenant.

#### Create User Flows and Custom Policies

Please refer to: [Tutorial: Create user flows in Azure Active Directory B2C](https://docs.microsoft.com/azure/active-directory-b2c/tutorial-create-user-flows)

#### Add External Identity Providers

Please refer to: [Tutorial: Add identity providers to your applications in Azure Active Directory B2C](https://docs.microsoft.com/azure/active-directory-b2c/tutorial-add-identity-providers)


#### Register the client app (active-directory-maui-b2c-v2)

1. Navigate to the [Azure portal](https://portal.azure.com) and select the **Azure Active Directory B2C** service.
1. Select the **App Registrations** blade on the left, then select **New registration**.
1. In the **Register an application page** that appears, enter your application's registration information:
    1. In the **Name** section, enter a meaningful application name that will be displayed to users of the app, for example `active-directory-maui-b2c-v2`.
    1. Under **Supported account types**, select **Accounts in this organizational directory only**
    1. Select **Register** to create the application.
1. In the **Overview** blade, find and note the **Application (client) ID**. You use this value in your app's configuration file(s) later in your code.
1. In the app's registration screen, select the **Authentication** blade to the left.
1. If you don't have a platform added, select **Add a platform** and select the **Public client (mobile & desktop)** option.
    1. In the **Redirect URIs** section, add **msal{ClientId}://auth**.
        The **ClientId** is the Id of the App Registration and can be found under **Overview/Application (client) ID**
    1. Click **Save** to save your changes.

##### Configure the client app (active-directory-maui-b2c-v2) to use your app registration

Open the project in your IDE (like Visual Studio or Visual Studio Code) to configure the code.

> In the steps below, "ClientID" is the same as "Application ID" or "AppId".

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

### Step 5: Run the sample!

Make sure the platform you configured is the same one you mark for build and deployment.

Clean the solution, build and run it:

- Click the sign-in button at the bottom of the application screen. Depending on whether or not there are conditional access policies applied to the user signing in, there will be two different experiences. If there are conditional access policies applied to the user signing in:
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
