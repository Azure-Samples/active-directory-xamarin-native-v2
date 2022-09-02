// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Android.Views;

using Microsoft.Identity.Client;
using Microsoft.Intune.Mam.Client.App;
using Microsoft.Intune.Mam.Client.Support.V7.App;
using Microsoft.Intune.Mam.Policy;
using Android.Support.V4.View;
using activedirectoryxamarinintune;
using Android.Content.PM;

namespace active_directory_xamarin_intune.Droid
{
    [Activity(Label = "Xamarin Intune Sample", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private const string AndroidRedirectURI = "msauth://com.yourcompany.xamarinintuneapp/EHyvOdXj4uLXJXDaOMy5lwANmp0="; // TODO - Replace with your redirectURI

        IIntuneMAMConnector _connector = new IntuneMAMConnector();

        protected override void OnMAMCreate(Bundle savedInstanceState)
        {
            base.OnMAMCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // configure platform specific params
            PlatformConfigImpl.Instance.RedirectUri = AndroidRedirectURI;
            PlatformConfigImpl.Instance.ParentWindow = this;

            // register IntuneMAMConnector
            Xamarin.Forms.DependencyService.Register<IIntuneMAMConnector, IntuneMAMConnector>();

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /// <summary>
        /// This is a callback to continue with the broker base authentication
        /// Info abour redirect URI: https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-client-application-configuration#redirect-uri
        /// </summary>
        /// <param name="requestCode">request code </param>
        /// <param name="resultCode">result code</param>
        /// <param name="data">intent of the actvity</param>
        protected override void OnMAMActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnMAMActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }
    }
}
