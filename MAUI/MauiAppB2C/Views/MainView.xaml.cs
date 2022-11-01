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
        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            try
            {
                AuthenticationResult result = await PCAWrapperB2C.Instance.AcquireTokenSilentAsync(PCAWrapperB2C.Instance.GetScopes());
            }
            catch (MsalUiRequiredException)
            {
                // This executes UI interaction to obtain token
                AuthenticationResult result = await PCAWrapperB2C.Instance.AcquireTokenInteractiveAsync(PCAWrapperB2C.Instance.GetScopes());
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
