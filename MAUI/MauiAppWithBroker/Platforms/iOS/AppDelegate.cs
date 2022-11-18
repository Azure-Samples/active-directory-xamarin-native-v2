// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;
using MAUI.MSALClient;
using Microsoft.Identity.Client;
using UIKit;

namespace MauiAppWithBroker
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // configure platform specific params
            PlatformConfig.Instance.RedirectUri = PublicClientSingleton.Instance.MSALClientHelper.AzureADConfig.iOSRedirectUri;
            PlatformConfig.Instance.ParentWindow = new UIViewController(); // iOS broker requires a view controller

            // Initialize MSAL after platformConfig is set
            IAccount existinguser = Task.Run(async () => await PublicClientSingleton.Instance.MSALClientHelper.InitializePublicClientAppForWAMBrokerAsync()).Result;

            return base.FinishedLaunching(application, launchOptions);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
        {
            if (AuthenticationContinuationHelper.IsBrokerResponse(null))
            {
                // Done on different thread to allow return in no time.
                _ = Task.Factory.StartNew(() => AuthenticationContinuationHelper.SetBrokerContinuationEventArgs(url));

                return true;
            }

            else if (!AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url))
            {
                return false;
            }

            return true;
        }
    }
}
