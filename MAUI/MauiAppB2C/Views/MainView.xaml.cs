// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MauiB2C.MSALClient;
using Microsoft.Identity.Client;

namespace MauiB2C.Views
{
    public partial class MainView : ContentPage
    {
        public MainView()
        {
            InitializeComponent();

            _ = Dispatcher.DispatchAsync(async () =>
            {
                SignInButton.IsEnabled = await PublicClientWrapperB2C.Instance.InitializCache();
            });

        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            try
            {
                AuthenticationResult result = await PublicClientWrapperB2C.Instance.AcquireTokenSilentAsync();
            }
            catch (MsalUiRequiredException)
            {
                // This executes UI interaction to obtain token
                AuthenticationResult result = await PublicClientWrapperB2C.Instance.AcquireTokenInteractiveAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception during signin", ex.Message, "OK").ConfigureAwait(false);
                return;
            }

            await Shell.Current.GoToAsync("scopeview");
        }
        protected override bool OnBackButtonPressed() { return true; }

    }
}
