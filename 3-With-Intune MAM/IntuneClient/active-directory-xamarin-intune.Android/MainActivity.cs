// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;

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

        IIntuneMAMConnector _connector = new IntuneMAMConnector();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // configure platform specific params
            PlatformConfigImpl.Instance.RedirectUri = AndroidRedirectURI;
            PlatformConfigImpl.Instance.ParentWindow = this;

            // register IntuneMAMConnector
            Xamarin.Forms.DependencyService.RegisterSingleton<IIntuneMAMConnector>(_connector);

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }
    }
}
