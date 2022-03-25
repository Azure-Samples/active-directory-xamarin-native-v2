# Integrate Microsoft Identity, the Microsoft Authenticator App (broker), and Microsoft Intune MAM SDK into a Xamarin Forms app using MSAL.NET.

This sample demonstrates how to access [Intune Mobile App Management (MAM)](https://docs.microsoft.com/en-us/mem/intune/apps/app-management) protected resources using MSAL.NET

## Overview
When a user attempts to access a MAM protected resource and if the device is not compliant, MSAL.NET will throw ```IntuneAppProtectionPolicyRequiredException```. The common code catches the exception and requests Intune MAM SDK to make the app compliant. Intune MAM SDK usage is platform specific. So this exception handling behavior is executed using Inversion of Control pattern, i.e. using a common interface with platform specific implementations.  

After the device becomes compliant, the App makes silent token request to obtain the token.  

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

### Step 1: Setup Azure Active Directory and Intune Portal
The app requires setup in Azure Active Directory to obtain your parameters. The setup instructions can be found [here](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Steps-to-create-config-for-MAM-(Conditional-access))


### Step 2:  Clone or download this repository

From your shell or command line:

```Shell
git clone https://github.com/Azure-Samples/active-directory-xamarin-native-v2.git
cd 2-With-broker
```

or download and exact the repository .zip file.

### Step 3:  Configure the Visual Studio project with your app coordinates
The app code is marked with TODO at places where parameters such as clientId, tenantID, redirect URI etc. need to be changed to correspond with the registered values. This is in the following files:
- PCAWrapper.cs
- AndroidManifest.xml
- Info.plist
- MainPage.xaml.cs
- MainActivity.cs
- AppDelegate.cs

### Step 4: Run the sample!
Run the sample on the platform of your choice.  
**Note**: If you run the sample with the existing clientId and other parameters, it will not function with your account. As user must be part of certain group in the Azure Active Directory as described in the setup.  

The details about platform specific implementation are [here](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/Protect-your-resources-in-iOS-and-Android-applications-using-Intune-MAM-and-MSAL.NET)