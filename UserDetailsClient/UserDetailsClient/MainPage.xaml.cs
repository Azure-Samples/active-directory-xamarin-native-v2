using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace UserDetailsClient
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void OnSignInSignOut(object sender, EventArgs e)
        {
            // on the UI thread now
            if (btnSignInSignOut.Text == "Sign in")
            {
                await SignInUserAsync().ConfigureAwait(false);

                // no longer on the UI thread becasuse of the ConfigureAwait(false)
                Device.BeginInvokeOnMainThread(
                    () => { btnSignInSignOut.Text = "Sign out"; });

            }
            else
            {
                await SignOutAllUsersAsync().ConfigureAwait(false);

                Device.BeginInvokeOnMainThread(
                    () => {
                        slUser.IsVisible = false;
                        btnSignInSignOut.Text = "Sign in";
                    });
            }
        }

        private async Task SignOutAllUsersAsync()
        {
            IEnumerable<IAccount> accounts = await App.PCA.GetAccountsAsync().ConfigureAwait(false);

            while (accounts.Any())
            {
                await App.PCA.RemoveAsync(accounts.FirstOrDefault());
                accounts = await App.PCA.GetAccountsAsync();
            }
        }

        private async Task SignInUserAsync()
        {
            AuthenticationResult authResult = null;
            IEnumerable<IAccount> accounts = await App.PCA.GetAccountsAsync().ConfigureAwait(false);

            // let's see if we have a user in our belly already
            try
            {
                IAccount firstAccount = accounts.FirstOrDefault();
                authResult = await App.PCA.AcquireTokenSilent(App.Scopes, firstAccount)
                                      .ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                // pop the browser for the interactive experience
                authResult = await App.PCA.AcquireTokenInteractive(App.Scopes)
                                          .WithParentActivityOrWindow(App.ParentActivity) // this is required for Android
                                          .ExecuteAsync();
            }

            await RefreshUserDataAsync(authResult.AccessToken).ConfigureAwait(false);
        }

        private async Task RefreshUserDataAsync(string token)
        {
            //get data from API
            HttpClient client = new HttpClient();
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
            HttpResponseMessage response = await client.SendAsync(message);
            string responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                JObject user = JObject.Parse(responseString);

                Device.BeginInvokeOnMainThread(() =>
                {
                    slUser.IsVisible = true;

                    lblDisplayName.Text = user["displayName"].ToString();
                    lblGivenName.Text = user["givenName"].ToString();
                    lblId.Text = user["id"].ToString();
                    lblSurname.Text = user["surname"].ToString();
                    lblUserPrincipalName.Text = user["userPrincipalName"].ToString();

                    // just in case
                    btnSignInSignOut.Text = "Sign out";
                });
            }
            else
            {
                await DisplayAlert("Something went wrong with the API call", responseString, "Dismiss");
            }
        }
    }
}
