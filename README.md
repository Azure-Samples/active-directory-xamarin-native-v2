---
services: active-directory
platforms: dotnet, xamarin
author: vibronet
---
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

### Step 5:  Run the sample

Clean the solution, rebuild the solution, and run it.


## Deploy this sample to Azure
Coming soon...
## About the code
Coming soon...
## More information
Coming soon...