// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using MAUIB2C.MSALClient;
using Microsoft.Identity.Client;

namespace MauiB2C;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        // configure platform specific params
        PlatformConfig.Instance.RedirectUri = $"msal{PublicClientSingleton.Instance.MSALClientHelper.AzureADB2CConfig.ClientId}://auth";
        PlatformConfig.Instance.ParentWindow = this;

        // Initialize MSAL and platformConfig is set
        _ = Task.Run(async () => await PublicClientSingleton.Instance.MSALClientHelper.InitializePublicClientAppAsync()).Result;
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
    }
}
