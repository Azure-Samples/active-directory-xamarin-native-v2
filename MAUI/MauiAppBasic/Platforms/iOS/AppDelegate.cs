// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;
using MAUI.MSALClient;
using Microsoft.Identity.Client;
using UIKit;

namespace MauiAppBasic
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        private const string iOSRedirectURI = "msauth.com.companyname.mauiappbasic://auth"; // TODO - Replace with your redirectURI
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // configure platform specific params
            PlatformConfig.Instance.RedirectUri = PublicClientSingleton.Instance.MSALClientHelper.AzureADConfig.iOSRedirectUri;

            // Initialize MSAL and platformConfig is set
            IAccount existinguser = Task.Run(async () => await PublicClientSingleton.Instance.MSALClientHelper.InitializePublicClientAppAsync()).Result;

            return base.FinishedLaunching(application, launchOptions);
        }
    }
}
