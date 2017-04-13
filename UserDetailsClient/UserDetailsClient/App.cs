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
        public static string ClientID = "a7d8cef0-4145-49b2-a91d-95c54051fa3f";
        public static string[] Scopes = { "User.Read" };
        public static string Username = string.Empty;

        public static UIParent UiParent = null;

      

        public App()
        {
            PCA = new PublicClientApplication(ClientID)
            {
#if __ANDROID__
                RedirectUri = "vibro://sampledomain/sampleapp",
#endif
            };
                        
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
