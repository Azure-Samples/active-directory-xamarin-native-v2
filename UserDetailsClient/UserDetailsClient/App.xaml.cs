using Microsoft.Identity.Client;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UserDetailsClient
{
    public partial class App : Application
    {
        public static IPublicClientApplication PCA = null;

        /// <summary>
        /// The ClientID is the Application ID found in the portal (https://go.microsoft.com/fwlink/?linkid=2083908). 
        /// You can use the below id however if you create an app of your own you should replace the value here.
        /// </summary>
        public static string ClientID = "a7d8cef0-4145-49b2-a91d-95c54051fa3f";

        public static string[] Scopes = { "User.Read" };
        public static string Username = string.Empty;

        /// <summary>
        /// The view, window or activity from where the interactive login happens. This is required in Android 
        /// to capture authentication result from the browser. 
        /// </summary>
        /// <remarks>
        /// Since this is a shared project, there is no reference to the Activity type, so keep this as an object.
        /// </remarks>
        public static object ParentActivity { get; set; }

        public App()
        {
            PCA = PublicClientApplicationBuilder.Create(ClientID)
              .WithRedirectUri($"msal{App.ClientID}://auth")
              .Build();

            InitializeComponent();

            MainPage = new MainPage();
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
