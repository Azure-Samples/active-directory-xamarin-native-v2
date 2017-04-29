---
services: active-directory
platforms: dotnet, xamarin
author: vibronet
---

# UWP - works #
# iOS - works#
# Android - works #
----

# Integrate Microsoft identity and the Microsoft Graph into a Xamarin forms app using MSAL
This is a simple Xamarin Forms app showcasing how to use MSAL to authenticate MSA and Azure AD via the converged MSA and Azure AD authentication endpoints, and access the Microsoft Graph with the resulting token.

For more information about how the protocols work in this scenario and other scenarios, see [Authentication Scenarios for Azure AD](http://go.microsoft.com/fwlink/?LinkId=394414).
For more information about Microsoft Graph, please visit [the Microsoft Graph homepage](https://graph.microsoft.io/en-us/).

## How To Run This Sample

To run this sample you will need:
- Visual Studio 2015
- An Internet connection
- At least one of the following accounts:
- - A Microsoft Account
- - An Azure AD account

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

1. Open the solution in Visual Studio 2015.
2. Open the `UserDatailsClient\App.cs` file.
3. Find the assignment for `public static string ClientID` and replace the value with the Application ID from the app registration portal, again in Step 2.

### Step 4:  Run the sample

Choose the platform you want to work on by setting the startup project in the Solution Explorer. Make sure that your platform of choice is marked for build and deploy in the Configuration Manager.
Clean the solution, rebuild the solution, and run it.
Click the sign-in button at the bottom of the application screen. On the sign-in screen, enter the name and password of a personal Microsoft account or a work/school account. The sample works exactly in the same way regardless of the account type you choose, apart from some visual differences in the authentication and consent experience.During the sign in process, you will be prompted to grant various permissions.   
Upon successful sign in and consent, the application screen will list some basic profile info for the authenticated user. Also, the button at the bottom of the screen will turn into a Sign out button.
Close the application and reopen it. You will see that the app retains access to the API and retrieves the user info right away, without the need to sign in again.
Sign out by clicking the Sign out button and confirm that you lose access to the API until the enxt interactive sign in.  

## Deploy this sample to Azure
Coming soon...
## About the code
The structure of the solution is straightforward. All the application logic and UX reside in UserDetailsClient (portable).
UserDetailsClient.Droid and UserDetailsClient.iOS both include a MainPageRenderer.cs class, which is used for assigning values at runtime to the PlatformParameters property of the main page. The PlatformParameters construct is used by MSAL for understanding at runtime on which platform it is running  - so that it can select the right authentication UX and token storage. Please note, MSAL does not need PlatformParameters for UWP apps.
UserDetailsClient.Droid requires one extra line of code to be added in the MainActivity.cs file:

```
AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode, data);

```
That line is used in `OnActivityResult` to ensure that the control goes back to MSAL once the interactive portion of the authentication flow ended.

The MSAL main primitive, `PublicClientApplication`, is initialized as a static variable in App.cs.
At application startup, the main page attempts to get a token without showing any UX - just in case a suitable token is already present in the cache from previous sessions. This is the code performeing that logic:
```
protected override async void OnAppearing()
{
    App.PCA.PlatformParameters = platformParameters;
    // let's see if we have a user in our belly already
    try
    {
        AuthenticationResult ar = await App.PCA.AcquireTokenSilentAsync(App.Scopes);
        RefreshUserData(ar.Token);
        btnSignInSignOut.Text = "Sign out";
    }
```
Note that this code is also used for assigning the previously mentioned MSAL's `PlatformParameters ` with the `platformParameters` value, itself assigned by the platform specific PageRenderer as discussed. 
If the attempt to obtain a token silently fails, we do nothing and display the screen with the sign in button.
When the sign in button is pressed, we execute the same logic - but using a method that shows interactive UX:

```
AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes);
```

The sign out logic is very simple. In this sample we have just one user, however we are demonstrating a more generic sign out logic that you can apply if you have multiple concurrent users               
```
foreach (var user in App.PCA.Users)
{
    user.SignOut();
}
```
## More information
For more information, please visit the [new documentation homepage for Microsoft identity](http://aka.ms/aaddevv2). 
