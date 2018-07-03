---
services: active-directory
platforms: dotnet, xamarin.iOS, xamarin.Android, UWP
author: jmprieur
level:  300
client: Xamarin 
service: Microsoft Graph 
endpoint: AAD V2
---
# Integrate Microsoft identity and the Microsoft Graph into a Xamarin forms app using MSAL

![Build badge](https://identitydivision.visualstudio.com/_apis/public/build/definitions/a7934fdd-dcde-4492-a406-7fad6ac00e17/32/badge)

## About this sample

This is a simple [Xamarin Forms](https://www.xamarin.com/visual-studio) app showcasing how to use MSAL.NET to:

1. authenticate users with Work or School accounts (AAD) or Microsoft personal accounts (MSA) and get an access token to
2. access the Microsoft Graph.

The Xamarin Forms application is provided for Xamarin.iOS, Xamarin.Android, and Xamarin.UWP

![Topology](./ReadmeFiles/Topology.png)

## How To Run this Sample

To run this sample you will need:

- [Visual Studio 2017](https://aka.ms/vsdownload) with the **Mobile development with .NET** [workload](https://www.visualstudio.com/vs/visual-studio-workloads/):
- An Internet connection
- At least one of the following accounts:
  - A Microsoft Account
  - An Azure AD account
- For [UWP](https://docs.microsoft.com/en-us/windows/uwp/get-started/whats-a-uwp), if you want to test the UWP application, you will have to add the SDK corresponding to your version of Windows 10 (or all the Windows 10 SDKs). See [Testing UWP applications on Windows 10](#testing-uwp-applications-on-windows-10)

You can get a Microsoft Account for free by choosing the Sign up option while visiting [https://www.microsoft.com/en-us/outlook-com/](https://www.microsoft.com/en-us/outlook-com/).
You can get an Office365 office subscription, which will give you an Azure AD account, at [https://products.office.com/en-us/try](https://products.office.com/en-us/try).

### Step 1:  Clone or download this repository

From your shell or command line:

`git clone https://github.com/Azure-Samples/active-directory-xamarin-native-v2.git`

> Given that the name of the sample is pretty long, and so are the name of the referenced NuGet packages, you might want to clone it in a folder close to the root of your hard drive, to avoid file size limitations on Windows.

### [OPTIONAL] Step 2:  Register the sample on the app registration portal

You can run the sample as is with its current settings, or you can optionally register it as a new application under your own developer account.
Create a new app at [apps.dev.microsoft.com](https://apps.dev.microsoft.com), or follow these [detailed steps](https://azure.microsoft.com/en-us/documentation/articles/active-directory-v2-app-registration/).  Make sure to:

- Copy down the **Application Id** assigned to your app, you'll need it in the next optional step.
- Add the **Native Application** platform for your app.

### [OPTIONAL] Step 3:  Configure the Visual Studio project with your app coordinates

1. Open the solution in Visual Studio 2017.
2. Open the `UserDatailsClient\App.cs` file.
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

- MSAL's main primitive for native clients, `PublicClientApplication`, is initialized as a static variable in App.cs (For details see [Client applications in MSAL.NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Client-Applications))

- At application startup, the main page attempts to get a token without showing any UX - just in case a suitable token is already present in the cache from previous sessions. This is the code performing that logic:

  ```CSharp
    protected override async void OnAppearing()
    {
        // let's see if we have a user in our belly already
        try
        {
            AuthenticationResult ar =
              await App.PCA.AcquireTokenSilentAsync(App.Scopes, App.PCA.Users.FirstOrDefault());
            RefreshUserData(ar.AccessToken);
            btnSignInSignOut.Text = "Sign out";
        }
        catch
        {
            // doesn't matter, we go in interactive more
            btnSignInSignOut.Text = "Sign in";
        }
    }
  ```

- If the attempt to obtain a token silently fails, we display a screen with the sign in button (at the bottom of the application).
- When the sign in button is pressed, we execute the same logic - but using a method that shows interactive UX:

  ```CSharp
  AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, App.UiParent);
  ```

- The `Scopes` parameter indicates the permissions the application needs to gain access to the data requested through subsequent web API call (in this sample, encapsulated in `RefreshUserData`).

The `UiParent` is used in Android to tie the authentication flow to the current activity, and is ignored on all other platforms. For more platform specific considerations, please see below.

- The sign out logic is very simple. In this sample we have just one user, however we are demonstrating a more generic sign out logic that you can apply if you have multiple concurrent users and you want to clear up the entire cache.
    ```CSharp
    foreach (var user in App.PCA.Users.ToArray())
    {
        App.PCA.Remove(user);
    }
    ```

### Platform specific considerations

The platform specific projects require only a couple of extra lines to accommodate for individual platform differences.

### Android specific considerations

The `UserDetailsClient.Droid` project requires two extra lines in the `MainActivity.cs` file.
In `OnActivityResult`, we need to add

```CSharp
AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
```

That line ensures that the control goes back to MSAL once the interactive portion of the authentication flow ended.

In `OnCreate`, we need to add the following assignment:

```CSharp
App.UiParent = new UIParent(this);
```

That code ensures that the authentication flows occur in the context of the current activity.

### iOS specific considerations

The `UserDetailsClient.iOS` project only requires one extra line, in `AppDelegate.cs`.
You need to ensure that the OpenUrl handler looks as the snippet below:

```CSharp
public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
{
    AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url, "");
    return true;
}
```

Once again, this logic is meant to ensure that once the interactive portion of the authentication flow is concluded, the flow goes back to MSAL.

Also, in order to make the token cache work and have the `AcquireTokenSilentAsync` work multiple steps must be followed :

1. Enable Keychain access in your `Entitlements.plist` file and specify in the **Keychain Groups** your bundle identifier.
1. In your project options, on iOS **Bundle Signing view**, select your `Entitlements.plist` file for the Custom Entitlements field.
1. When signing a certificate, make sure XCode uses the same Apple Id. 

### UWP specific considerations

You can set the `UseCorporateNework` boolean to `true` to benefit from windows integrated authentication (and therefore SSO with the user signed-in with the operating system) if this user is signed-in with an account in a federated Azure AD tenant. This leverages WAB (Web Authentication Broker). Setting this property to true assumes that the application developer has enabled Windows Integrated Authentication (WIA) in the application. For this, in the `Package.appxmanifest` for your UWP application, in the Capabilities tab, enable the following capabilities:

- Enterprise Authentication
- Private Networks (Client & Server)
- Shared User Certificate

WIA is not enabled by default because applications requesting the Enterprise Authentication or Shared User Certificates capabilities require a higher level of verification to be accepted into the Windows Store, and not all developers may wish to perform the higher level of verification.

## Troubleshooting

### Some projects don't load in Visual Studio

This might be because you have not installed all the required components from Visual Studio. you need'll need to add the **Mobile development with .NET** [workload](https://www.visualstudio.com/vs/visual-studio-workloads/), in the Visual Studio Installer.

### The project you want is not built

you need to right click on the visual studio solution, choose **Configuration Properties** > **Configuration** and make sure that you check the projects and configuration you want to build (and deploy)

### Testing UWP applications on Windows 10

- you might want to right click on the `UserDetailsClient.UWP (Universal Windows)` project and, in the **Application** tab, change the Universal Windows **Target** and **Min Version** depending on the SDK you have installed on your machine.
- To install more Windows 10 SDKs, run **Visual Studio Installer**, choose your Visual Studio installation, click on **Modify**, and in the **Individual components** tab, in the **SDKs, Libraries, and frameworks** section, make sure you check all the Windows 10 SDK versions that you need.

### On Windows 10 store apps, you cannot sign-in with your windows hello PIN

If sign-in with your work or school account and your organization requires conditional access, you are asked to provide a certificate:

- If you did not enabled UWP specific considerations above, you will get this error:
    ```Text
    No valid client certificate found in the request. No valid certificates found in the user's certificate store. Please try again choosing a different authentication method.
    ```
- On Windows 10 desktop UWP application, if you enabled the settings described in [UWP specific considerations](#UWP-specific-considerations), the list of certificates is presented, however if you choose to use your PIN, the PIN window is never presented. This is a known limitation with Web authentication broker in UWP applications running on Windows 10 (this works fine on Windows Phone 10). As a work around, you will need to click on the **sign in with other options** link and then choose **Sign-in with a username and password instead**, provide your password and go through the phone authentication.

## More information

For more information, please visit:

- For more information on acquiring tokens with MSAL.NET, please visit [MSAL.NET's conceptual documentation](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki), in particular:
  - [PublicClientApplication](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Client-Applications#publicclientapplication)
  - [Recommended call pattern in public client applications](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/AcquireTokenSilentAsync-using-a-cached-token#recommended-call-pattern-in-public-client-applications)
  - [Acquiring tokens interactively in public client application flows](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Acquiring-tokens-interactively)
- To undestand more about the AAD V2 endpoint see http://aka.ms/aaddevv2
- For more information about how the protocols work in this scenario and other scenarios, see [Authentication Scenarios for Azure AD](http://go.microsoft.com/fwlink/?LinkId=394414).
- For more information about Microsoft Graph, please visit [the Microsoft Graph homepage](https://graph.microsoft.io/en-us/)
