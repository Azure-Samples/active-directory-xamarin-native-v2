// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using MAUI.MSALClient;
using Microsoft.Identity.Client;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MauiAppWithBroker.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            // configure redirect URI for your application
            PlatformConfig.Instance.RedirectUri = PublicClientSingleton.Instance.MSALClientHelper.AzureADConfig.RedirectURI;

            // Initialize MSAL after platform config is set
            IAccount existinguser = Task.Run(async () => await PublicClientSingleton.Instance.MSALClientHelper.InitializePublicClientAppForWAMBrokerAsync()).Result;
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);


            // The brokers need the window handle specifically to operate and this event is earliest instance we have one provided. 
            var app = MauiAppWithBroker.App.Current;
            PlatformConfig.Instance.ParentWindow = ((MauiWinUIWindow)app.Windows[0].Handler.PlatformView).WindowHandle;
        }
    }
}
