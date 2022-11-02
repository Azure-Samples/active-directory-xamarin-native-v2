﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using MauiAppWithBroker.MSALClient;
using Microsoft.Identity.Client;

namespace MauiAppWithBroker
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private const string AndroidRedirectURI = "msauth://com.companyname.mauiappwithbroker/{Your package signature}"; // TODO - Replace with your redirectURI

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // configure platform specific params
            PlatformConfig.Instance.RedirectUri = AndroidRedirectURI;
            PlatformConfig.Instance.ParentWindow = this;
        }

        /// <summary>
        /// This is a callback to continue with the broker base authentication
        /// Info abour redirect URI: https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-client-application-configuration#redirect-uri
        /// </summary>
        /// <param name="requestCode">request code </param>
        /// <param name="resultCode">result code</param>
        /// <param name="data">intent of the actvity</param>
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }
    }
}
