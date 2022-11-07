// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MAUI.MSALClient;
using Microsoft.Identity.Client;

namespace MauiAppBasic.Views
{
    public partial class MainView : ContentPage
    {
        private MSALClientHelper MSALClientHelper;

        public MainView()
        {
            InitializeComponent();

            this.MSALClientHelper = new MSALClientHelper("MauiAppBasic.appsettings.json");
            // Initializes the Public Client app and loads any already signed in user from the token cache
            var cachedUserAccount = Task.Run(async () => await MSALClientHelper.InitializePublicClientAppForWAMBrokerAsync()).Result;

            _ = Dispatcher.DispatchAsync(async () =>
            {
                //SignInButton.IsEnabled = await PCAWrapper.Instance.InitializCache();
                //SignInButton.IsEnabled = true;//await PCAWrapper.Instance.InitializCache();
                if (cachedUserAccount == null)
                {
                    SignInButton.IsEnabled = true;
                }
            });
        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            //try
            //{
            //    PublicClientWrapper.Instance.UseEmbedded = this.useEmbedded.IsChecked;

            //    AuthenticationResult result = await PublicClientWrapper.Instance.AcquireTokenSilentAsync();
            //}
            //catch (MsalUiRequiredException)
            //{
            //    // This executes UI interaction to obtain token
            //    AuthenticationResult result = await PublicClientWrapper.Instance.AcquireTokenInteractiveAsync();
            //}
            //catch (Exception ex)
            //{
            //    await DisplayAlert("Exception during signin", ex.Message, "OK").ConfigureAwait(false);
            //    return;
            //}

            await Shell.Current.GoToAsync("userview");
        }
        protected override bool OnBackButtonPressed() { return true; }

    }
}
