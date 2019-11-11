---
page_type: sample
languages:
- csharp
- powershell
products:
- azure-active-directory
description: "You have a Xamarin mobile application and you want it to consume Microsoft Graph or your own Web Api using Microsoft Identity Platform to acquire tokens."
urlFragment: active-directory-xamarin-native-v2
---

# A Xamarin mobile application using Microsoft identity platform (formerly Azure AD v2.0)

[![Build status](https://identitydivision.visualstudio.com/IDDP/_apis/build/status/AAD%20Samples/.NET%20client%20samples/CI%20of%20Azure-Samples%20--%20xamarin-native-v2)](https://identitydivision.visualstudio.com/IDDP/_build/latest?definitionId=32)

## About this sample

### Scenario

You have a mobile application and you want it to consume Microsoft Graph or your own Web Api using **Microsoft Identity Platform** to acquire tokens.

### Structure of the repository

This repository contains a progressive tutorial made of two parts:

Sub folder                    | Description
----------------------------- | -----------
[1-Basic](https://github.com/Azure-Samples/active-directory-xamarin-native-v2/tree/master/1-Basic) | This sample application shows how to use the Microsoft identity platform endpoint to sign-in a user interactively and display their profile </p> ![Topology](./1-Basic/ReadmeFiles/Topology.png)
[2-With-broker](https://github.com/Azure-Samples/active-directory-xamarin-native-v2/tree/master/2-With-broker)  | This incremental chapter adds support for the broker (Microsoft Authenticator), enabling more scenarios like device related conditional access and SSO. This chapter is only available for Xamarin.iOS for the moment </p>  ![Topology](./2-With-broker/ReadmeFiles/Topology.png)

## How to run this sample

To run this sample, you'll need:

- [Visual Studio 2019](https://aka.ms/vsdownload) or just the [.NET Core SDK](https://www.microsoft.com/net/learn/get-started)
- An Internet connection
- A Windows machine (necessary if you want to run the app on Windows)
- An OS X machine (necessary if you want to run the app on Mac)
- A Linux machine (necessary if you want to run the app on Linux)
- An Azure Active Directory (Azure AD) tenant. For more information on how to get an Azure AD tenant, see [How to get an Azure AD tenant](https://azure.microsoft.com/en-us/documentation/articles/active-directory-howto-tenant/)
- A user account in your Azure AD tenant. This sample will not work with a Microsoft account (formerly Windows Live account). Therefore, if you signed in to the [Azure portal](https://portal.azure.com) with a Microsoft account and have never created a user account in your directory before, you need to do that now.

### Step 1:  Clone or download this repository

From your shell or command line:

```Shell
git clone https://github.com/Azure-Samples/https://github.com/Azure-Samples/active-directory-xamarin-native-v2.git
```

or download and exact the repository .zip file.

> Given that the name of the sample is pretty long, and so are the name of the referenced NuGet packages, you might want to clone it in a folder close to the root of your hard drive, to avoid file size limitations on Windows.

Then navigate to the sub-folder of your choice, [1-Basic](./1-Basic) or [2-With-broker](./2-With-broker)

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
- [Use Microsoft Authenticator or Microsoft Intune Company Portal on Xamarin applications](https://docs.microsoft.com/azure/active-directory/develop/msal-net-use-brokers-with-xamarin-apps)
