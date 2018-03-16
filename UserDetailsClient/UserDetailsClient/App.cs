using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace UserDetailsClient
{
    public class App : Application
    {
        public static PublicClientApplication PCA = null;
        public static string ClientID = "6b923147-7261-4ddb-a9eb-90233746a414";
        public static string[] Scopes = { "User.Read" };
        public static string Username = string.Empty;

        public static UIParent UiParent = null;

      

        public App()
        {
            // default redirectURI; each platform specific project will have to override it with its own
            PCA = new PublicClientApplication(ClientID);
                        
            MainPage = new NavigationPage(new UserDetailsClient.MainPage());        
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
