---
services: active-directory
platforms: dotnet, xamarin
author: jmprieur
---


# Integrate Microsoft identity and the Microsoft Graph into a Xamarin forms app using MSAL
This is a simple Xamarin Forms app showcasing how to use MSAL to authenticate MSA and Azure AD via the converged MSA and Azure AD authentication endpoints, and access the Microsoft Graph with the resulting token.

For more information about how the protocols work in this scenario and other scenarios, see [Authentication Scenarios for Azure AD](http://go.microsoft.com/fwlink/?LinkId=394414).
For more information about Microsoft Graph, please visit [the Microsoft Graph homepage](https://graph.microsoft.io/en-us/).

## How To Run This Sample

To run this sample you will need:
- Visual Studio 2017
- An Internet connection
- At least one of the following accounts:
- A Microsoft Account
- An Azure AD account

You can get a Microsoft Account for free by choosing the Sign up option while visiting [https://www.microsoft.com/en-us/outlook-com/](https://www.microsoft.com/en-us/outlook-com/). 
You can get an Office365 office subscription, which will give you an Azure AD account, at [https://products.office.com/en-us/try](https://products.office.com/en-us/try). 


### Step 1:  Clone or download this repository

From your shell or command line:

`git clone https://github.com/Azure-Samples/active-directory-xamarin-native-v2.git`

### [OPTIONAL] Step 2:  Register the sample on the app registration portal

You can run the sample as is with its current settings, or you can optionally register it as a new application under your own developer account. 
Create a new app at [apps.dev.microsoft.com](https://apps.dev.microsoft.com), or follow these [detailed steps](https://azure.microsoft.com/en-us/documentation/articles/active-directory-v2-app-registration/).  Make sure to:

- Copy down the **Application Id** assigned to your app, you'll need it in the next optional step.
- Add the **Mobile** platform for your app.

### [OPTIONAL] Step 3:  Configure the Visual Studio project with your app coordinates

1. Open the solution in Visual Studio 2017.
2. Open the `UserDatailsClient\App.cs` file.
3. Find the assignment for `public static string ClientID` and replace the value with the Application ID from the app registration portal, again in Step 2.

#### [OPTIONAL] Step 3a: Configure the iOS project with your apps' return URI
1. Open the `UserDetailsClient.iOS\AppDelegate.cs` file.
2. Locate the `App.PCA.RedirectUri` assignment, and change it to assign the string `"msal<Application Id>://auth"` where `<Application Id>` is the identifier you copied in step 2
3. Open the `UserDetailsClient.iOS\info.plist` file in a text editor (opening it in Visual Studio won't work for this step as you need to edit the text)
4. In the URL types, section, add an entry for the authorization schema used in your redirectUri.
```
    <key>CFBundleURLTypes</key>
       <array>
     <dict>
       <key>CFBundleTypeRole</key>
       <string>Editor</string>
       <key>CFBundleURLName</key>
       <string>com.yourcompany.UserDetailsClient</string>
       <key>CFBundleURLSchemes</key>
       <array>
     <string>msala[APPLICATIONID]</string>
       </array>
     </dict>
       </array> 
```
where `[APPLICATIONID]` is the identifier you copied in step 2. Save the file.
#### [OPTIONAL] Step 3b: Configure the Android project with your return URI

1. Open the `UserDetailsClient.Droid\MainActivity.cs` file.
2. Locate the `App.PCA.RedirectUri` assignment, and change it to assign the string `"msal<Application Id>://auth"` where `<Application Id>` is the identifier you copied in step 2
3. Open the `UserDetailsClient.Droid\Properties\AndroidManifest.xml`
4. Add or modify the `<application>` element as in the following
```
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
Clean the solution, rebuild the solution, and run it.
Click the sign-in button at the bottom of the application screen. On the sign-in screen, enter the name and password of a personal Microsoft account or a work/school account. The sample works exactly in the same way regardless of the account type you choose, apart from some visual differences in the authentication and consent experience.During the sign in process, you will be prompted to grant various permissions.   
Upon successful sign in and consent, the application screen will list some basic profile info for the authenticated user. Also, the button at the bottom of the screen will turn into a Sign out button.
Close the application and reopen it. You will see that the app retains access to the API and retrieves the user info right away, without the need to sign in again.
Sign out by clicking the Sign out button and confirm that you lose access to the API until the enxt interactive sign in.

#### Running in an Android Emulator
MSAL in Android requires support for Chrome Custom Tabs for displaying authentication prompts.
Not every emulator image comes with Chrome on board: please refer to [this document](https://github.com/Azure-Samples/active-directory-general-docs/blob/master/AndroidEmulator.md) for instructions on how to ensure that your emulator supports the features required by MSAL. 

## About the code
The structure of the solution is straightforward. All the application logic and UX reside in UserDetailsClient (portable).
MSAL's main primitive for native clients, `PublicClientApplication`, is initialized as a static variable in App.cs.
At application startup, the main page attempts to get a token without showing any UX - just in case a suitable token is already present in the cache from previous sessions. This is the code performing that logic:
```
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

If the attempt to obtain a token silently fails, we do nothing and display the screen with the sign in button.
When the sign in button is pressed, we execute the same logic - but using a method that shows interactive UX:

```
AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, App.UiParent);
```
The `Scopes` parameter indicates the permissions the application needs to gain access to the data requested throuhg subsequent web API call (in this sample, encapsulated in `RefreshUserData`). 
The UiParent is used in Android to tie the authentication flow to the current activity, and is ignored on all other platforms. For more platform specific considerations, please see below.

The sign out logic is very simple. In this sample we have just one user, however we are demonstrating a more generic sign out logic that you can apply if you have multiple concurrent users and you want to clear up the entire cache.               
```
foreach (var user in App.PCA.Users)
{
    App.PCA.Remove(user);
}
```

### Android specific considerations
The platform specific projects require only a couple of extra lines to accommodate for individual platform differences.

UserDetailsClient.Droid requires one two extra lines in the `MainActivity.cs` file.
In `OnActivityResult`, we need to add

```
AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);

```
That line ensures that the control goes back to MSAL once the interactive portion of the authentication flow ended.

In `OnCreate`, we need to add the following assignment:
```
App.UiParent = new UIParent(Xamarin.Forms.Forms.Context as Activity); 
```
That code ensures that the authentication flows occur in the context of the current activity.  


### iOS specific considerations

UserDetailsClient.iOS only requires one extra line, in AppDelegate.cs.
You need to ensure that the OpenUrl handler looks as ine snippet below:
```
public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
{
    AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url, "");
    return true;
}
```
Once again, this logic is meant to ensure that once the interactive portion of the authentication flow is concluded, the flow goes back to MSAL.


## More information
For more information, please visit the [new documentation homepage for Microsoft identity](http://aka.ms/aaddevv2). 
