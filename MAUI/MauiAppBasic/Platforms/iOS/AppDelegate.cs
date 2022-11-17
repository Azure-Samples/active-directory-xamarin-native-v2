// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;
using MAUI.MSALClient;
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
            PlatformConfig.Instance.RedirectUri = iOSRedirectURI;

            // Initialize MSAL and platformConfig is set
            var existinguser = Task.Run(async () => await PublicClientWrapper.Instance.MSALClientHelper.InitializePublicClientAppAsync()).Result;

            return base.FinishedLaunching(application, launchOptions);
        }
    }
}
