// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MAUI.MSALClient;
using Microsoft.Identity.Client;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MauiAppBasic.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        private const string RedirectURIWindows = "http://localhost";

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            // configure redirect URI for your application
            PlatformConfig.Instance.RedirectUri = PublicClientSingleton.Instance.MSALClientHelper.AzureADConfig.RedirectURI;

            // Initialize MSAL
            IAccount existinguser = Task.Run(async () => await PublicClientSingleton.Instance.MSALClientHelper.InitializePublicClientAppAsync()).Result;
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
        }
    }
}