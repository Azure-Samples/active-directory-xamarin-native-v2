---
page_type: sample
name: A .NET MAUI app using MSAL.NET to sign-in users and calling MS Graph Api
description: Integrate Microsoft Identity for a B2C tenant into a MAUI app using MSAL
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
- client: MAUI (iOS, Android, WinUI)
- service: Microsoft Graph
---

# Integrate Microsoft Identity for a B2C tenant into a MAUI app using MSAL

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

### Step 2: Navigate to project folder

```console
cd MAUI/MauiAppB2C
```

### Step 3: Register the sample application(s) in your tenant

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

1. Open the `appsettings.json` file.
1. Find the key `Instance` and replace the existing value with the instance url of your B2C tenant.
1. Find the key `Domain` and replace the existing value with the domain of your B2C tenant.
1. Find the key `TenantId` and replace the existing value with your Azure AD tenant/directory ID.
1. Find the key `ClientId` and replace the existing value with the application ID (clientId) of `active-directory-maui-b2c-v2` app copied from the Azure portal.
1. Find the key `SignUpSignInPolicyId` and replace the existing value with the sign-in/sign-up policy you wish to use.
1. Find the key `ResetPasswordPolicyId` and replace the existing value with the password reset policy you wish to use (optional).
1. Find the key `EditProfilePolicyId` and replace the existing value with the edit profile policy you wish to use (optional).
1. Find the key `CacheFileName` and replace the existing value with the name of the cache file you wish to use with WinUI caching (not used in Android nor iOS).
1. Find the key `CacheDir` and replace the existing value with the directory path storing cache file you wish to use with WinUI caching (not used in Android nor iOS).
1. Find the key `Scopes` and replace the existing value with the scopes (space separated) you wish to use in your application.

1. Open the `Platforms\Android\MsalActivity.cs` file.
1. Find the key `[REPLACE THIS WITH THE CLIENT ID OF YOUR APP]` and replace the existing value with the application ID (clientId) of `active-directory-maui-b2c-v2` app copied from the Azure portal.

1. Open the `Platforms\Android\AndroidManifest.xml` file.
1. Find the key `[REPLACE THIS WITH THE CLIENT ID OF YOUR APP]` and replace the existing value with the application ID (clientId) of `active-directory-maui-b2c-v2` app copied from the Azure portal.

1. Open the `Platforms\iOS\AppDelegate.cs` file.
1. Find the key `[REPLACE THIS WITH THE CLIENT ID OF YOUR APP]` and replace the existing value with the application ID (clientId) of `active-directory-maui-b2c-v2` app copied from the Azure portal.

### Step 4: Running the sample

Choose the platform you want to work on by setting the startup project in the Solution Explorer. Make sure that your platform of choice is marked for build and deploy in the Configuration Manager.
Clean the solution, rebuild the solution, and run it.

## Explore the sample

- Click the sign-in button at the bottom of the application screen.
- On the sign-in screen, enter the name and password of a personal Microsoft account or a work/school account. The sample works exactly in the same way regardless of the account type you choose, apart from some visual differences in the authentication and consent experience. During the sign in process, you will be prompted to grant various permissions (to allow the application to access your data).
- Upon successful sign in and consent, the application screen will display the main page.
- On WinUI you can close the application and reopen it. You will see that the app retains access to the API and retrieves the user info right away, without the need to sign in again.
- Sign out by clicking the sign out button.

## We'd love your feedback!

Were we successful in addressing your learning objective? Consider taking a moment to [share your experience with us](https://forms.microsoft.com/Pages/DesignPageV2.aspx?subpage=design&m2=1&id=v4j5cvGGr0GRqy180BHbR9p5WmglDttMunCjrD00y3NURVlKVzVFTEdPUTVCQThZRlhVUTJDNklYRS4u).

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

- If the attempt to obtain a token silently fails, a screen with the sign in button (at the bottom of the application) is displayed.
- When the sign in button is pressed, we execute the same logic - but using a method that shows interactive UX:

```CSharp
return await this.PublicClientApplication.AcquireTokenInteractive(scopes)
    .WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
    .ExecuteAsync()
    .ConfigureAwait(false);
```

- The `Scopes` parameter indicates the permissions the application needs to gain access to the data requested through subsequent web API call.

- The sign out logic is very simple. In this sample we have just one user, however we are demonstrating a more generic sign out logic that you can apply if you have multiple concurrent users and you want to clear up the entire cache.

```CSharp
await this.PublicClientApplication.RemoveAsync(user).ConfigureAwait(false);
```

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

This logic is meant to ensure that once the interactive portion of the authentication flow is concluded by the Authenticator app, the flow goes back to MSAL.

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
