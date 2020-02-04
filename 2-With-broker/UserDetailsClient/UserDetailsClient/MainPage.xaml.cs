using Microsoft.Identity.Client;
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
        private static bool UseBroker { get; set; } = false;
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnSignInSignOut(object sender, EventArgs e)
        {
            UseBroker = false;
            AcquireTokenAsync().ConfigureAwait(false);
        }

        public static void CreatePublicClient()
        {
            var builder = PublicClientApplicationBuilder
                .Create(App.ClientID);
                
            if (UseBroker)
            {
                builder.WithBroker();
                builder = builder.WithIosKeychainSecurityGroup("com.microsoft.adalcache");
                
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        builder = builder.WithRedirectUri(App.BrokerRedirectUriOnAndroid);
                        break;
                    case Device.iOS:
                    default:
                        builder = builder.WithRedirectUri(App.BrokerRedirectUriOnIos);
                        break;
                }
            }
            else
            {
                builder = builder.WithRedirectUri($"msal{App.ClientID}://auth");
            }

            App.PCA = builder.Build();
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

        private void btnSignInSignOutWithBroker_Clicked(object sender, EventArgs e)
        {
            UseBroker = true;
            CreatePublicClient();

            AcquireTokenAsync().ConfigureAwait(false);
        }

        private async Task AcquireTokenAsync()
        {
            AuthenticationResult authResult = null;
            IEnumerable<IAccount> accounts = await App.PCA.GetAccountsAsync();
            try
            {
                if (btnSignInSignOut.Text == "Sign in")
                {
                    try
                    {
                        IAccount firstAccount = accounts.FirstOrDefault();
                        authResult = await App.PCA.AcquireTokenSilent(App.Scopes, firstAccount)
                                              .ExecuteAsync();
                    }
                    catch (MsalUiRequiredException ex)
                    {
                        try
                        {
                            authResult = await App.PCA.AcquireTokenInteractive(App.Scopes)
                                                      .WithParentActivityOrWindow(App.ParentWindow)
                                                      .WithUseEmbeddedWebView(true)
                                                      .ExecuteAsync();
                        }
                        catch (Exception ex2)
                        {
                            await DisplayAlert("Acquire token interactive failed. See exception message for details: ", ex2.Message, "Dismiss");
                        }
                    }

                    if (authResult != null)
                    {
                        var content = await GetHttpContentWithTokenAsync(authResult.AccessToken);
                        UpdateUserContent(content);
                        Device.BeginInvokeOnMainThread(() => { btnSignInSignOut.Text = "Sign out"; });
                    }
                }
                else
                {
                    while (accounts.Any())
                    {
                        await App.PCA.RemoveAsync(accounts.FirstOrDefault());
                        accounts = await App.PCA.GetAccountsAsync();
                    }

                    slUser.IsVisible = false;
                    Device.BeginInvokeOnMainThread(() => { btnSignInSignOut.Text = "Sign in"; });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Authentication failed. See exception message for details: ", ex.Message, "Dismiss");
            }
        }
    }
}