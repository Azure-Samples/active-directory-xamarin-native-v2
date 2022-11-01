// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MauiAppBasic.MSALClient;
using MauiAppBasic.ViewModels;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace MauiAppBasic.Views
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
                PCAWrapper.Instance.UseEmbedded = this.useEmbedded.IsChecked;

                AuthenticationResult result = await PCAWrapper.Instance.AcquireTokenSilentAsync(PCAWrapper.Instance.GetScopes());
            }
            catch (MsalUiRequiredException)
            {
                // This executes UI interaction to obtain token
                AuthenticationResult result = await PCAWrapper.Instance.AcquireTokenInteractiveAsync(PCAWrapper.Instance.GetScopes());
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
