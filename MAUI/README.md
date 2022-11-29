---
page_type: sample
languages:
- csharp
- powershell
products:
- azure-active-directory
description: "You have a MAUI application and you want it to consume Microsoft Graph or your own Web Api using Microsoft Identity Platform to acquire tokens."
urlFragment: active-directory-xamarin-native-v2
---

# A MAUI mobile or desktop application using Microsoft identity platform

[![Build status](https://identitydivision.visualstudio.com/IDDP/_apis/build/status/AAD%20Samples/.NET%20client%20samples/CI%20of%20Azure-Samples%20--%20xamarin-native-v2)](https://identitydivision.visualstudio.com/IDDP/_build/latest?definitionId=32)

## About this sample

### Scenario

You have a mobile or Windows desktop application and you want it to consume either Microsoft Graph or your own Web API using the **Microsoft Identity Platform** to acquire tokens.

### Structure of the repository

This repository contains a three-part tutorial - a basic scenario, a more advanced with broker scenario and a scenario using Azure Active Directory B2C. Choose the one that best suits your scenario, or go through both to understand the differences between the implementations.

Sub folder                    | Description
----------------------------- | -----------
[MauiAppBasic](https://github.com/Azure-Samples/active-directory-xamarin-native-v2/tree/master/MAUI/MauiAppBasic) | This sample app shows how to use the Microsoft identity platform endpoint to sign-in a user interactively and display their profile </p> ![Topology](./MauiAppBasic/ReadmeFiles/topology.png)
[MauiAppWithBroker](https://github.com/Azure-Samples/active-directory-xamarin-native-v2/tree/master/MAUI/MauiAppWithBroker)  | This chapter adds additional support for the broker (Microsoft Authenticator), which enables more complex scenarios, like device related conditional access and SSO. </p>  ![Topology](./MauiAppWithBroker/ReadmeFiles/Topology.png)
[MauiAppB2C](https://github.com/Azure-Samples/active-directory-xamarin-native-v2/tree/master/MAUI/MauiAppB2C)  | This sample app shows how to use the Microsoft identity platform endpoint to sign-in a user interactively with Azure Active Directory B2C and display their access permissions </p>  ![Topology](./MauiAppB2C/ReadmeFiles/Topology.png)

## How to run this sample for MAUI

To run this sample, you'll need:

- [Visual Studio](https://aka.ms/vsdownload). Install or update Visual Studio with the following workloads:
  - Universal Windows Platform Development
  - Mobile Development with .Net
  
  Then from the "Individual Components" tab, make sure these additional items are selected:
  - Android SDK setup (API level 27)
  - Windows 10 SDK (10.0.17134.0)
  - Android SDK level 27 (oreo) and 28 (pie), and Android SDK build tools 27.0.3 are also required. These are not installed through the VS Installer, so instead use the Android SDK Manager (Visual Studio > Tools > Android > Android SDK Manager…)
- An Internet connection
- A Windows or OS X machine (necessary if you want to run the app on their respective platforms)
- An Azure Active Directory (Azure AD) tenant. For more information on how to get an Azure AD tenant, see [How to get an Azure AD tenant](https://azure.microsoft.com/en-us/documentation/articles/active-directory-howto-tenant/)
  - If you wish to run the B2C sample you will need an[Azure Active Directory B2C Tenant](https://learn.microsoft.com/azure/active-directory-b2c/tutorial-create-tenant)
- A user account in your Azure AD tenant. This sample will not work with a Microsoft account (formerly Windows Live account). Therefore, if you signed in to the [Azure portal](https://portal.azure.com) with a Microsoft account and have never created a user account in your directory before, you need to do that now.
- Setup [MAUI](https://learn.microsoft.com/dotnet/maui/get-started/installation?tabs=vswin&view=net-maui-7.0). This will require Visual Studio on PC, and on a Mac Machine.

### Step 1:  Clone or download this repository

From your shell or command line:

```Shell
git clone https://github.com/Azure-Samples/active-directory-xamarin-native-v2.git
```

or download and exact the repository .zip file.

> Given that the name of the sample is pretty long, and so are the name of the referenced NuGet packages, you might want to clone it in a folder close to the root of your hard drive, to avoid file size limitations on Windows.

Then navigate to the sub-folder of your choice, [MauiAppBasic](./MAUI/MauiAppBasic), [MauiAppWithBroker](./MAUI/MauiAppWithBroker) or [MauiAppB2C](./MAUI/MauiAppB2C)

## Community Help and Support

Use [Stack Overflow](http://stackoverflow.com/questions/tagged/msal) to get support from the community.
Ask your questions on Stack Overflow first and browse existing issues to see if someone has asked your question before.
Make sure that your questions or comments are tagged with [`msal` `dotnet`].

If you find a bug in the sample, please raise the issue on [GitHub Issues](../../issues).

If you find a bug in msal.Net, please raise the issue on [MSAL.NET GitHub Issues](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/issues).

To provide a recommendation, visit the following [User Voice page](https://feedback.azure.com/forums/169401-azure-active-directory).

## Contributing

If you'd like to contribute to this sample, see [CONTRIBUTING.MD](/CONTRIBUTING.md).

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information, see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## More information

For more information, see MSAL.NET's conceptual documentation:

- [Mobile application scenario landing page](https://docs.microsoft.com/azure/active-directory/develop/scenario-mobile-overview)
- [Quickstart: Register an application with the Microsoft identity platform](https://docs.microsoft.com/azure/active-directory/develop/quickstart-register-app)
- [Quickstart: Configure a client application to access web APIs](https://docs.microsoft.com/azure/active-directory/develop/quickstart-configure-app-access-web-apis)