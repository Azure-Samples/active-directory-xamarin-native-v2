// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MAUI.MSALClient;

namespace MauiAppWithBroker.Views
{
    public partial class MainView : ContentPage
    {
        private MSALClientHelper MSALClientHelper;

        public MainView()
        {

            this.MSALClientHelper = new MSALClientHelper();
            // Initializes the Public Client app and loads any already signed in user from the token cache
            var cachedUserAccount = Task.Run(async () => await MSALClientHelper.InitializePublicClientAppForWAMBrokerAsync()).Result;

            _ = Application.Current.Dispatcher.Dispatch(async () =>
            {
                if (cachedUserAccount == null)
                {
                    SignInButton.IsEnabled = true;
                }
            });

            InitializeComponent();
        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            //try
            //{
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
