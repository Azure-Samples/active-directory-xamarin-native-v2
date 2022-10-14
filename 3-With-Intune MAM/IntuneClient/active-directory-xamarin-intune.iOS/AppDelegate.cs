// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using activedirectoryxamarinintune;
using Foundation;
using UIKit;

using Microsoft.Identity.Client;
using Microsoft.Intune.MAM;

namespace active_directory_xamarin_intune.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private const string iOSRedirectURI = "msauth.com.yourcompany.XamarinIntuneApp://auth"; // TODO - Replace bundleId with your redirectURI

        MainIntuneMAMComplianceDelegate _mamComplianceDelegate = new MainIntuneMAMComplianceDelegate();
        MainIntuneMAMEnrollmentDelegate _mamEnrollmentDelegate = new MainIntuneMAMEnrollmentDelegate();
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            // configure platform specific params
            PlatformConfigImpl.Instance.RedirectUri = iOSRedirectURI;
            PlatformConfigImpl.Instance.ParentWindow = new UIViewController(); // iOS broker requires a view controller

            // register the connector
            Xamarin.Forms.DependencyService.Register<IIntuneMAMConnector, IntuneMAMConnector>();

            // register the delegate for IntuneMAMCompliance manager
            IntuneMAMComplianceManager.Instance.Delegate = _mamComplianceDelegate;
            IntuneMAMEnrollmentManager.Instance.Delegate = _mamEnrollmentDelegate;

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url,
                             string sourceApplication,
                             NSObject annotation)
        {
            if (AuthenticationContinuationHelper.IsBrokerResponse(sourceApplication))
            {
                AuthenticationContinuationHelper.SetBrokerContinuationEventArgs(url);
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
