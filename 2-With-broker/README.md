---
services: active-directory
platforms: dotnet, xamarin.iOS, xamarin.Android, UWP
author: jennyf19
level:  300
client: Xamarin 
service: Microsoft Graph 
endpoint: AAD V2
---
# Integrate Microsoft identity, the Microsoft Authenticator App (broker), and Microsoft Graph into a Xamarin forms app using MSAL

![Build badge](https://identitydivision.visualstudio.com/_apis/public/build/definitions/a7934fdd-dcde-4492-a406-7fad6ac00e17/32/badge)

## About this sample

This is a [Xamarin Forms](https://www.xamarin.com/visual-studio) app showcasing how to use MSAL.NET to:

1. authenticate users with Work or School accounts (AAD) or Microsoft personal accounts (MSA) using the Microsoft Authenticator App to get an access token to
2. access the Microsoft Graph.

The Xamarin Forms application is provided for Xamarin.iOS and Xamarin.Android

![Topology](./ReadmeFiles/Topology.png)

## What is the broker?

On Android and iOS, brokers enable:

- Single Sign On (SSO). Your users won't need to sign-in to each application.
- Device identification. By accessing the device certificate which was created on the device when it was workplace joined.
- Application identification verification. When an application calls the broker, it passes its redirect url, and the broker verifies it.

## How To Run this Sample

To run this sample you will need:

- [Visual Studio 2019](https://aka.ms/vsdownload) with the **Mobile development with .NET** [workload](https://www.visualstudio.com/vs/visual-studio-workloads/):
- An Internet connection
- At least one of the following accounts:
  - A Microsoft Account
  - An Azure AD account

You can get a Microsoft Account for free by choosing the Sign up option while visiting [https://www.microsoft.com/en-us/outlook-com/](https://www.microsoft.com/en-us/outlook-com/).
You can get an Office365 office subscription, which will give you an Azure AD account, at [https://products.office.com/en-us/try](https://products.office.com/en-us/try).

### Step 1:  Clone or download this repository

From your shell or command line:

```Shell
git clone https://github.com/Azure-Samples/active-directory-xamarin-native-v2.git
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
1. Select **Save**.
1. In the list of pages for the app, select **API permissions**
   - Click the **Add a permission** button and then,
   - Ensure that the **Microsoft APIs** tab is selected
   - In the *Commonly used Microsoft APIs* section, click on **Microsoft Graph**
   - In the **Delegated permissions** section, ensure that the right permissions are checked: **User.Read**. Use the search box if necessary.
   - Select the **Add permissions** button

### [OPTIONAL] Step 3:  Configure the Visual Studio project with your app coordinates

In the steps below, "ClientID" is the same as "Application ID" or "AppId".
1. Open the solution in Visual Studio 2017.
2. Open the `UserDetailsClient\App.cs` file.
3. Find the assignment for `public static string ClientID` and replace the value with the Application ID from the app registration portal, created in Step 2.

#### [OPTIONAL] Step 3a: Configure the iOS project with your apps' return URI

1. Open the `UserDetailsClient.iOS\AppDelegate.cs` file.
2. Open the `UserDetailsClient.iOS\info.plist` file in a text editor (opening it in Visual Studio won't work for this step as you need to edit the text)
3. In the URL types section, add an entry for the authorization schema used in your redirectUri:

```Xml
    <key>CFBundleURLTypes</key>
       <array>
     <dict>
       <key>CFBundleTypeRole</key>
       <string>Editor</string>
       <key>CFBundleURLName</key>
       <string>com.yourcompany.UserDetailsClient</string>
       <key>CFBundleURLSchemes</key>
       <array>
     <string>msal[APPLICATIONID]</string>
       </array>
     </dict>
       </array>
```

where `[APPLICATIONID]` is the identifier you copied in step 2. Save the file.

#### [OPTIONAL] Step 3b: Configure the Android project with your return URI

1. Open the `UserDetailsClient.Droid\MainActivity.cs` file.
2. Open the `UserDetailsClient.Droid\Properties\AndroidManifest.xml`
3. Add or modify the `<application>` element as in the following:

```Xml
    <application>
    <activity android:name="microsoft.identity.client.BrowserTabActivity">
      <intent-filter>
    <action android:name="android.intent.action.VIEW" />
    <category android:name="android.intent.category.DEFAULT" />
    <category android:name="android.intent.category.BROWSABLE" />
    <data android:scheme="msal[APPLICATIONID]" android:host="auth" />
      </intent-filter>
    </activity>
      </application>
```

where `[APPLICATIONID]` is the identifier you copied in step 2. Save the file.

### Step 4: Set up integration with the Authenticator App (iOS broker)
Follow the steps below to enable your Xamarin.iOS app to talk with the [Microsoft Authenticator](https://itunes.apple.com/us/app/microsoft-authenticator/id983156458) app. 

#### Step 1: Enable broker support
Broker support is enabled on a per-PublicClientApplication basis. It is disabled by default. You must use the `WithBroker()` parameter (set to true by default) when creating the PublicClientApplication through the PublicClientApplicationBuilder.

```CSharp
var app = PublicClientApplicationBuilder
                .Create(ClientId)
                .WithBroker()
                .WithReplyUri(redirectUriOnIos) // $"msauth.{Bundle.Id}://auth" (see step 6 below)
                .Build();
```

#### Step 2: Update AppDelegate to handle the callback
When MSAL.NET calls the broker, the broker will, in turn, call back to your application through the `OpenUrl` method of the `AppDelegate` class. Since MSAL will wait for the response from the broker, your application needs to cooperate to call MSAL.NET back. You do this by updating the `AppDelegate.cs` file to override the below method.

```CSharp
public override bool OpenUrl(UIApplication app, NSUrl url, 
                             string sourceApplication,
                             NSObject annotation)
{
    if (AuthenticationContinuationHelper.IsBrokerResponse(sourceApplication))
    {
         AuthenticationContinuationHelper.SetBrokerContinuationEventArgs(url);
         return true;
    }
    
    else if (!AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url))
    {	            
         return false;	              
    }
	
    return true;	 
}	        
```

This method is invoked every time the application is launched and is used as an opportunity to process the response from the broker and complete the authentication process initiated by MSAL.NET.

#### Step 3: Set a UIViewController()
Still in `AppDelegate.cs`, you will need to set an object window. Normally, with Xamarin iOS, you do not need to set the object window, but in order to send and receive responses from broker, you will need an object window. 

To do this, you will need to do two things. 
1) In `AppDelegate.cs`, set the `App.RootViewController` to a new `UIViewController()`. This will make sure there is a UIViewController with the call to the broker. If it is not set correctly, you may get this error:
`"uiviewcontroller_required_for_ios_broker":"UIViewController is null, so MSAL.NET cannot invoke the iOS broker. See https://aka.ms/msal-net-ios-broker"`
2) On the AcquireTokenInteractive call, use the `.WithParentActivityOrWindow(App.RootViewController)` and pass in the reference to the object window you will use.

**For example:**

In `App.cs`:
```CSharp
   public static object RootViewController { get; set; }
```
In `AppDelegate.cs`:
```CSharp
   LoadApplication(new App());
   App.RootViewController = new UIViewController();
```
In the Acquire Token call:
```CSharp
result = await app.AcquireTokenInteractive(scopes)
             .WithParentActivityOrWindow(App.RootViewController)
             .ExecuteAsync();
```

#### Step 4: Register a URL Scheme
MSAL.NET uses URLs to invoke the broker and then return the broker response back to your app. To finish the round trip, you need to register a URL scheme for your app in the `Info.plist` file.

The `CFBundleURLSchemes` name must include `msauth.` as a prefix, followed by your `CFBundleURLName`.

`$"msauth.(BundleId)"`

**For example:**
`msauth.com.yourcompany.xforms`

**Note** This will become part of the RedirectUri used for uniquely identifying your app when receiving the response from the broker.

```CSharp
 <key>CFBundleURLTypes</key>
    <array>
      <dict>
        <key>CFBundleTypeRole</key>
        <string>Editor</string>
        <key>CFBundleURLName</key>
        <string>com.yourcompany.xforms</string>
        <key>CFBundleURLSchemes</key>
        <array>
          <string>msauth.com.yourcompany.xforms</string>
        </array>
      </dict>
    </array>
```

#### Step 5: LSApplicationQueriesSchemes
MSAL uses `canOpenURL:` to check if the broker is installed on the device. In iOS 9, Apple locked down what schemes an application can query for. 

**Add** **`msauthv2`** to the `LSApplicationQueriesSchemes` section of the `Info.plist` file.

```CSharp 
<key>LSApplicationQueriesSchemes</key>
    <array>
      <string>msauthv2</string>
      <string>msauthv3</string>
    </array>
```

#### Step 6: Register your RedirectUri in the application portal
Using the broker adds an extra requirement on your redirectUri. The redirectUri _**must**_ have the following format:
```CSharp
$"msauth.{BundleId}://auth"
```
**For example:**
```CSharp
public static string redirectUriOnIos = "msauth.com.yourcompany.XForms://auth"; 
```
**You'll notice the RedirectUri matches the `CFBundleURLSchemes` name you included in the `Info.plist` file.**

#### Step 7: make sure the redirect URI is registered with your app

This Redirect URI needs to be registered on the app registration portal (https://portal.azure.com) as a valid redirect URI for your application. 

Time to run the app!

### Step 5:  Run the sample

Since you configured the iOS broker above, choose ioS as the platform you want to work on by setting the startup project in the Solution Explorer. Make sure that iOS is marked for build and deploy in the Configuration Manager.
Clean the solution, rebuild the solution, and run it:

- Click the sign-in with broker button at the bottom of the application screen. 
- The Authenticator App will open, or ask you to install it from the App Store. 
- In the Authenticator App, either select an already existing account, or add a new one.
- If adding a new one, on the sign-in screen, enter the name and password of a personal Microsoft account or a work/school account. The authenticator app works exactly in the same way regardless of the account type you choose, apart from some visual differences in the authentication and consent experience. During the sign in process, you will be prompted to grant various permissions (to allow the application to access your data).
- Upon successful sign in and consent, the application screen will list some basic profile info for the authenticated user. Also, the button at the bottom of the screen will turn into a Sign out button.
- Close the application and reopen it. You will see that the app retains access to the API and retrieves the user info right away, without the need to sign in again.
- Sign out by clicking the Sign out button and confirm that you lose access to the API until the next interactive sign in.

## About the code

The structure of the solution is straightforward. All the application logic and UX reside in ``UserDetailsClient (portable)`` project.

- MSAL's main primitive for native clients, `PublicClientApplication`, is initialized as a static variable in App.cs (For details see [Client applications in MSAL.NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Client-Applications))

  ```CSharp
  PCA = PublicClientApplicationBuilder.Create(ClientID)
                                      .WithRedirectUri($"msal{App.ClientID}://auth")
                                      .Build();
  ```

- At application startup, the main page (`UserDetailsClient/MainPage.xaml.cs`) attempts to get a token without showing any UX - just in case a suitable token is already present in the cache from previous sessions. This is the code performing that logic:

  ```CSharp
  try
  {
   IAccount firstAccount = accounts.FirstOrDefault();
   authResult = await App.PCA.AcquireTokenSilent(App.Scopes, firstAccount)
                             .ExecuteAsync();
   /* display info*/
  }
  catch (MsalUiRequiredException ex)
  {
   try
   {
    authResult = await App.PCA.AcquireTokenInteractive(App.Scopes, App.ParentWindow)
                              .ExecuteAsync();

    /* display info*/
   }
  }
  ```

- If the attempt to obtain a token silently fails, we display a screen with the sign in button (at the bottom of the application).
- When the sign in button is pressed, we execute the same logic - but using a method that shows interactive UX:

  ```CSharp
  AuthenticationResult ar = await App.PCA.AcquireTokenInteractive(App.Scopes, App.ParentWindow);
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

The `UserDetailsClient.iOS` project only requires one extra line, in `AppDelegate.cs`.
You need to ensure that the OpenUrl handler looks as the snippet below:

```CSharp
public override bool OpenUrl(UIApplication app, NSUrl url, 
                             string sourceApplication,
                             NSObject annotation)
{
    if (AuthenticationContinuationHelper.IsBrokerResponse(sourceApplication))
    {
         AuthenticationContinuationHelper.SetBrokerContinuationEventArgs(url);
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

This might be because you have not installed all the required components from Visual Studio. you need'll need to add the **Mobile development with .NET** [workload](https://www.visualstudio.com/vs/visual-studio-workloads/), in the Visual Studio Installer.

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
- To undestand more about the AAD V2 endpoint see http://aka.ms/aaddevv2
- For more information about how the protocols work in this scenario and other scenarios, see [Authentication Scenarios for Azure AD](http://go.microsoft.com/fwlink/?LinkId=394414).
- For more information about Microsoft Graph, please visit [the Microsoft Graph homepage](https://graph.microsoft.io/en-us/)