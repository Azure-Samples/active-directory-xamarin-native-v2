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
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnSignInSignOut(object sender, EventArgs e)
        {            
            CreatePublicClient(false);
            AcquireTokenAsync().ConfigureAwait(false);
        }

        public static void CreatePublicClient(bool useBroker)
        {
            var builder = PublicClientApplicationBuilder
                .Create(App.ClientID);
                
            if (useBroker)
            {                
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        builder = builder.WithRedirectUri(App.BrokerRedirectUriOnAndroid);
                        break;
                    case Device.iOS:
                        builder = builder.WithIosKeychainSecurityGroup("com.microsoft.adalcache");
                        builder = builder.WithRedirectUri(App.BrokerRedirectUriOnIos);
                        break;
                    case Device.UWP:
                        builder = builder.WithExperimentalFeatures();

                        // See also UserDetailsClient.UWP project in MainPage.xml.cs
                        // To get the redirect URI that you need to register in your app
                        // registration of a shape similar to:
                        // ms-appx-web://microsoft.aad.brokerplugin/S-1-15-2-3163378744-4254380357-4090943427-3442740072-2185909759-2930900273-1603380124
                        builder.WithDefaultRedirectUri();
                        break;
                }

                builder.WithBroker();
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
            CreatePublicClient(true);

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

                        // On UWP, you can set firstAccount = PublicClientApplication.OperatingSystemAccount if you
                        // want to sign-in THE account signed-in with Windows 
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