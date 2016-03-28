using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace UserDetailsClient
{
    public partial class MainPage : ContentPage
    {
        public IPlatformParameters platformParameters { get; set; }
        public MainPage()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            App.PCA.PlatformParameters = platformParameters;
            // let's see if we have a user in our belly already
            try
            {
                AuthenticationResult ar = await App.PCA.AcquireTokenSilentAsync(App.Scopes);
                RefreshUserData(ar.Token);
                btnSignInSignOut.Text = "Sign out";
            }
            catch
            {
                // doesn't matter, we go in interactive more
                btnSignInSignOut.Text = "Sign in";
            }
        }
        async void OnSignInSignOut(object sender, EventArgs e)
        {
            if (btnSignInSignOut.Text == "Sign in")
            {
                AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes);
                RefreshUserData(ar.Token);
                btnSignInSignOut.Text = "Sign out";
            }
            else
            {
                foreach (var user in App.PCA.Users)
                {
                    user.SignOut();
                }
                slUser.IsVisible = false;
                btnSignInSignOut.Text = "Sign in";
            }
        }

        public async void RefreshUserData(string token)
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

                slUser.IsVisible = true;
                lblDisplayName.Text = "displayName  " + user["displayName"].ToString();
                lblGivenName.Text = "givenName  " + user["givenName"].ToString();
                lblId.Text = "id  " + user["id"].ToString();               
                lblSurname.Text = "surname  " + user["surname"].ToString();
                lblUserPrincipalName.Text = "userPrincipalName  " + user["userPrincipalName"].ToString();

                // just in case
                btnSignInSignOut.Text = "Sign out";

               
            }
            else
            {
                DisplayAlert("Something went wrong with the API call", responseString, "Dismiss");
            }
        }
    }

    
}
