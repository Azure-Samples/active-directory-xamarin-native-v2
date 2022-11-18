// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MAUI.MSALClient;
using Microsoft.Identity.Client;

namespace MauiAppBasic.Views
{
    public partial class MainView : ContentPage
    {
        public MainView()
        {
            InitializeComponent();

            //var assembly = Assembly.GetExecutingAssembly();
            //using var stream = assembly.GetManifestResourceStream("MauiAppBasic.appsettings.json");
            //IConfiguration AppConfiguration = new ConfigurationBuilder()
            //    .AddJsonStream(stream)
            //    .Build();

            //AzureADConfig azureADConfig  = AppConfiguration.GetSection("AzureAD").Get<AzureADConfig>();

            //this.MSALClientHelper = new MSALClientHelper(azureADConfig);

            // Initializes the Public Client app and loads any already signed in user from the token cache
            IAccount cachedUserAccount = Task.Run(async () => await PublicClientSingleton.Instance.MSALClientHelper.FetchSignedInUserFromCache()).Result;

            _ = Dispatcher.DispatchAsync(async () =>
            {
                if (cachedUserAccount == null)
                {
                    SignInButton.IsEnabled = true;
                }
                else
                {
                    await Shell.Current.GoToAsync("userview");
                }
            });
        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            // Sign-in the user
            PublicClientSingleton.Instance.UseEmbedded = this.useEmbedded.IsChecked;
            await PublicClientSingleton.Instance.AcquireTokenSilentAsync();

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

        protected override bool OnBackButtonPressed()
        { return true; }
    }
}