// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MauiAppBasic.MSALClient;
using Microsoft.Identity.Client;

namespace MauiAppBasic.Views
{
    public partial class MainView : ContentPage
    {
        public MainView()
        {
            InitializeComponent();

            _ = Dispatcher.DispatchAsync(async () =>
            {
                //SignInButton.IsEnabled = await PCAWrapper.Instance.InitializCache();
                SignInButton.IsEnabled = true;//await PCAWrapper.Instance.InitializCache();
            });
        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            try
            {
                PCAWrapper.Instance.UseEmbedded = this.useEmbedded.IsChecked;

                AuthenticationResult result = await PCAWrapper.Instance.AcquireTokenSilentAsync();
            }
            catch (MsalUiRequiredException)
            {
                // This executes UI interaction to obtain token
                AuthenticationResult result = await PCAWrapper.Instance.AcquireTokenInteractiveAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception during signin", ex.Message, "OK").ConfigureAwait(false);
                return;
            }

            await Shell.Current.GoToAsync("userview");
        }
        protected override bool OnBackButtonPressed() { return true; }

    }
}
