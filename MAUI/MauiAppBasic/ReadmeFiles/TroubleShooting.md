## Troubleshooting

### Some projects don't load in Visual Studio

This might be because you have not installed all the required components from Visual Studio. You need to add the **.NET Mutli-platform App UI development** [workload](https://learn.microsoft.com/en-us/visualstudio/install/modify-visual-studio?view=vs-2022), in the Visual Studio Installer.

### The project you want is not built

you need to right click on the visual studio solution, choose **Configuration Properties** > **Configuration** and make sure that you check the projects and configuration you want to build (and deploy)

## Moving from sample to production

Samples favor simple code structures that show you how to use MSAL. Samples do not showcase best practices and patterns, nor do they make use of other libraries.

- Consider using [dependency injection](https://medium.com/syncfusion/learn-how-to-use-dependency-injection-in-net-maui-800f1bf9bc4d) for the `IPublicClientApplication` 
- Consider wrapping the construction of the `IPublicClientApplication` and `AcquireToken*` in another class to make testing possible. Mocking the existing builder pattern for creating `IPublicClientApplication` and `AcquireTokenInteractiveParameterBuilder` is not possible (we've tried).
- MSAL will generally let HTTP exceptions propagate. Consider using [Maui.Networking.Connectivity](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/communication/networking?tabs=windows) to detect situations where the network is down in order to provide a better error message to the user. 