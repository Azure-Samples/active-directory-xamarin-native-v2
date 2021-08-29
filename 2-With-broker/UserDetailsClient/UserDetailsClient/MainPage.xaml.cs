using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Helper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace UserDetailsClient
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            CreatePublicClient();
        }

        public static void CreatePublicClient()
        {
            string redirectUri = null;
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    redirectUri = App.BrokerRedirectUriOnAndroid;
                    break;
                case Device.iOS:
                    redirectUri = App.BrokerRedirectUriOnIos;
                    break;
            }

            PCAHelper.Init(App.ClientID, App.Scopes, redirectUri);
            if (Device.RuntimePlatform == Device.UWP)
            {
                PCAHelper.Instance.PCABuilder.WithExperimentalFeatures();
                PCAHelper.Instance.PCABuilder.WithDefaultRedirectUri();
            }
            PCAHelper.Instance.PCABuilder.WithBroker();
        }

        private void UpdateUserContent(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                JObject user = JObject.Parse(content);

                slUser.IsVisible = true;

                Device.BeginInvokeOnMainThread(() =>
                {
                    lblDisplayName.Text = user["displayName"].ToString();
                    lblGivenName.Text = user["givenName"].ToString();
                    lblId.Text = user["id"].ToString();
                    lblSurname.Text = user["surname"].ToString();
                    lblUserPrincipalName.Text = user["userPrincipalName"].ToString();

                    btnSignInSignOut.Text = "Sign out";
                });
            }
        }

        public async Task<string> GetHttpContentWithTokenAsync(string token)
        {
            try
            {
                //get data from API
                HttpClient client = new HttpClient();
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.SendAsync(message);
                string responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch (Exception ex)
            {
                await DisplayAlert("API call to graph failed: ", ex.Message, "Dismiss");
                return ex.ToString();
            }
        }

        private async void btnSignInSignOutWithBroker_Clicked(object sender, EventArgs e)
        {
            AuthenticationResult authResult = null;
            IEnumerable<IAccount> accounts = await PCAHelper.Instance.PCA.GetAccountsAsync();
            try
            {
                if (PCAHelper.Instance.AuthResult == null)
                {
                    authResult = await PCAHelper.Instance.EnsureAuthenticatedAsync(account: accounts.FirstOrDefault());

                    if (authResult != null)
                    {
                        var content = await GetHttpContentWithTokenAsync(authResult.AccessToken);
                        UpdateUserContent(content);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            btnSignInSignOut.Text = "Sign out";
                        });
                    }
                }
                else
                {
                    await PCAHelper.Instance.SignOutAsync();

                    slUser.IsVisible = false;
                    Device.BeginInvokeOnMainThread(() => { btnSignInSignOut.Text = "Sign in with Broker"; });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Authentication failed. See exception message for details: ", ex.Message, "Dismiss");
            }
        }
    }
}