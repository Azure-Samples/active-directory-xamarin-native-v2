# Integrate Microsoft Identity, the Microsoft Authenticator App (broker), and Microsoft Intune MAM SDK into a Xamarin Forms app using MSAL.NET.

This sample demonstrates how to access [Intune Mobile App Management (MAM)](https://docs.microsoft.com/en-us/mem/intune/apps/app-management) protected resources using MSAL.NET

## Overview
When a user attempts to access a MAM protected resource and if the device is not compliant, MSAL.NET will throw ```IntuneAppProtectionPolicyRequiredException```. The common code catches the exception and requests Intune MAM SDK to make the app compliant. Intune MAM SDK usage is platform specific. So this exception handling behavior is executed using Inversion of Control pattern, i.e. using a common interface with platform specific implementations.  

After the device becomes compliant, the App makes a silent token request to obtain the token.  

The sample has a code tour that shows the workflow of the app.  You can open the source code in [VS Code](https://code.visualstudio.com/). Get the [CodeTour Extension](https://marketplace.visualstudio.com/items?itemName=vsls-contrib.codetour) and run the steps. It will walk you through the code flow.  

**Note:** The sample showcases only the scenario when user completes the compliance request. Handling of other scenarios when user does not satisfy the compliance request is up to the business requirements of the application.

## How To Run this Sample

### Prerequisites
To run this sample you will need:
- [Visual Studio 2019](https://aka.ms/vsdownload) with the **Mobile development with .NET** [workload](https://www.visualstudio.com/vs/visual-studio-workloads/):
- An Internet connection
- At least one of the following accounts:
  - A Microsoft Account in Premium tier- you can get a free account by visiting [https://www.microsoft.com/en-us/outlook-com/](https://www.microsoft.com/en-us/outlook-com/).
  - An Azure AD account - you can get a free trial Office 365 account by visiting [https://products.office.com/en-us/try](https://products.office.com/en-us/try).
- Setup [Xamarin.iOS for Visual Studio](https://docs.microsoft.com/en-us/xamarin/ios/get-started/installation/windows/introduction-to-xamarin-ios-for-visual-studio) (if you want to run the iOS app)

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
1. Open the solution in Visual Studio 2019.
2. Open the `UserDetailsClient\App.cs` file.
3. Find the assignment for `public static string ClientID` and replace the value with the Application ID from the app registration portal, created in Step 2.

#### [iOS specific] Step 3a: Configure the iOS project with your apps' return URI

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

#### [Android specific] Step 3b: Configure the Android project with your return URI

1. Open the `UserDetailsClient.Droid\MsalActivity.cs` file.
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
string mobileRedirectUri;
switch (Device.RuntimePlatform)
{
    case Device.Android:
        mobileRedirectUri = App.BrokerRedirectUriOnAndroid;
        break;
    case Device.iOS:
        mobileRedirectUri = App.BrokerRedirectUriOniOS;
        break;
}

var app = PublicClientApplicationBuilder
                .Create(ClientId)
                .WithBroker()
                .WithReplyUri(mobileRedirectURI) 
                .Build();
```
This step has already been completed in the code [here.](https://github.com/Azure-Samples/active-directory-xamarin-native-v2/blob/3d877ba0ded3e644610654f6ad1ca7abb30f9e5b/2-With-broker/UserDetailsClient/UserDetailsClient/MainPage.xaml.cs#L34)

#### Step 4b: Configure application to handle the authentication callback
When MSAL.NET calls the broker on iOS, the broker will, in turn, call back to your application through the `OpenUrl` method of the `AppDelegate` class. Since MSAL will wait for the response from the broker, your application needs to cooperate to call MSAL.NET back. You do this by updating the `AppDelegate.cs` file to override the below method. This step is already completed in the AppDelegate.cs file, [found here.](https://github.com/Azure-Samples/active-directory-xamarin-native-v2/blob/3d877ba0ded3e644610654f6ad1ca7abb30f9e5b/2-With-broker/UserDetailsClient/UserDetailsClient.iOS/AppDelegate.cs#L33)

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

Similar to iOS, Android needs a way of ensuring that it is able to capture the authentication result from the interactive activity. This is done by overriding the `OnActivityResult` method in `MainActivity.cs`. (This step is already completed [in MainActivity.cs](https://github.com/Azure-Samples/active-directory-xamarin-native-v2/blob/3d877ba0ded3e644610654f6ad1ca7abb30f9e5b/2-With-broker/UserDetailsClient/UserDetailsClient.Droid/MainActivity.cs#L27))

```csharp
protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
{
  base.OnActivityResult(requestCode, resultCode, data);
  AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
}
```

This method is invoked every time the application is launched and is used as an opportunity to process the response from the broker and complete the authentication process initiated by MSAL.NET.

#### [iOS only] Step 4c: Set a UIViewController()
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

#### [iOS only] Step 4d: Register a URL Scheme
MSAL.NET uses URLs to invoke the broker and then return the broker response back to your app. To finish the round trip, you need to register a URL scheme for your app in the `Info.plist` file.

The `CFBundleURLSchemes` name must include `msauth.` as a prefix, followed by your `CFBundleURLName`. This should be the first entry.

`$"msauth.(BundleId)"`
It also requires additional entry for 
`msauth.com.microsoft.intunemam`

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
        <string>msauth.com.microsoft.intunemam</string>
        <key>CFBundleURLSchemes</key>
        <array>
          <string>msauth.com.yourcompany.xforms</string>
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

#### [iOS only] Step 4f: IntuneMAMSettings
Please update the clientId in the following settings.  
If you have a single tenant app, you need to modify the ADALAuthority value replacing orgnizations with the tenantId

```CSharp 
	<key>IntuneMAMSettings</key>
	<dict>
		<key>ADALAuthority</key>
		<string>https://login.microsoftonline.com/organizations</string>
		<key>ADALClientId</key>
		<string>TODO - Your Client Id</string>
		<key>ADALRedirectUri</key>
		<string>msauth.com.yourcompany.XamarinIntuneApp://auth</string>
	</dict>
```

#### Step 4g: Register your RedirectUri in the application portal
Using the broker adds an extra requirement on your redirectUri. The redirectUri _**must**_ have the following format:

**For iOS:**
```CSharp
$"msauth.{BundleId}://auth"
```
**Example for iOS:**
```CSharp
public static string redirectUriOnIos = "msauth.com.yourcompany.XForms://auth"; 
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

### Step 5: Setup Azure Active Directory and Intune Company Portal
The app alos requires setup in the Intune Company Portal. The setup instructions can be found [here](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Steps-to-create-config-for-MAM-(Conditional-access))

### Step 6:  Configure the Visual Studio project with your app coordinates
The app code is marked with TODO at places where parameters such as clientId, tenantID, redirect URI etc. need to be changed to correspond with the registered values. This is in the following files:
- PCAWrapper.cs
- AndroidManifest.xml
- Info.plist
- MainPage.xaml.cs
- MainActivity.cs
- AppDelegate.cs

### Step 7: Run the sample!
Run the sample on the platform of your choice.  
**Note**: If you run the sample with the existing clientId and other parameters, it will not function with your account. As user must be part of certain group in the Azure Active Directory as described in the setup.  

Make sure the platform you configured is the same one you mark for build and deployment.

Clean the solution, build and run it:

- Click the sign-in with broker button at the bottom of the application screen. Depending on whether or not there are conditional access policies applied to the user signing in, there will be two different experiences. If there are conditional access policies applied to the user signing in:
    - The Authenticator App will open, or ask you to install it from the App Store. 
    - In the Authenticator App, either select an already existing account, or add a new one.
    - If adding a new one, on the sign-in screen, enter the name and password of a personal Microsoft account or a work/school account. The authenticator app works exactly in the same way regardless of the account type you choose, apart from some visual differences in the authentication and consent experience. During the sign in process, you will be prompted to grant various permissions (to allow the application to access your data).
    - Upon successful sign in and consent, the application screen will list some basic profile info for the authenticated user. Also, the button at the bottom of the screen will turn into a Sign out button.
    - Close the application and reopen it. You will see that the app retains access to the API and retrieves the user info right away, without the need to sign in again.
    - Sign out by clicking the Sign out button and confirm that you lose access to the API until the next interactive sign in. This will unenroll the app.
- If there are no conditional access policies applied to the user signing in, signing in with a broker is not required, so you will see the typical sign-in UI flow.
- If there is an MFA policy in addition to the Inutne policy, it will require user to sign-in twice for the first time.


The details about platform specific implementation are [here](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Protect-your-resources-in-iOS-and-Android-applications-using-Intune-MAM-and-MSAL.NET)