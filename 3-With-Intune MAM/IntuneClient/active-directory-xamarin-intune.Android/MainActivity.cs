// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using activedirectoryxamarinintune;
using Android.Content;
using Microsoft.Identity.Client;

namespace active_directory_xamarin_intune.Droid
{
    [Activity(Label = "Xamarin Intune Sample", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private const string AndroidRedirectURI = "msauth://com.yourcompany.xamarinintuneapp/JjmT52ASxa1Lz55s+qPPgxb5xeo="; // TODO - Replace with your redirectURI

        protected override void OnMAMCreate(Bundle savedInstanceState)
        {
            base.OnMAMCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // configure platform specific params
            PlatformConfigImpl.Instance.RedirectUri = AndroidRedirectURI;
            PlatformConfigImpl.Instance.ParentWindow = this;

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
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
