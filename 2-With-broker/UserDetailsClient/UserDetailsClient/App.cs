using Microsoft.Identity.Client;
using Xamarin.Forms;

namespace UserDetailsClient
{
    public class App : Application
    {
        public static IPublicClientApplication PCA = null;

        /// <summary>
        /// The ClientID is the Application ID found in the portal (https://go.microsoft.com/fwlink/?linkid=2083908). 
        /// You can use the below id however if you create an app of your own you should replace the value here.
        /// </summary>
        public static string ClientID = "4a1aa1d5-c567-49d0-ad0b-cd957a47f842"; // ms-identity-samples-testing
        public const string BrokerRedirectUriOnIos = "msauth.com.yourcompany.UserDetailsClient://auth";

        //The redirect uri on Android will need to be created based on the signature of the .APK used to sign it. 
        //This means that it will be different depending on where this sample is run because Visual Studio creates
        //a unique signing key for debugging purposes on every machine. You can figure out what that signature will be by running the following commands
        //- For Windows: `keytool -exportcert -alias androiddebugkey -keystore %HOMEPATH%\.android\debug.keystore | openssl sha1 -binary | openssl base64`
        //- For Mac: `keytool -exportcert -alias androiddebugkey -keystore ~/.android/debug.keystore | openssl sha1 -binary | openssl base64`
        //For more details, please visit https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-net-use-brokers-with-xamarin-apps
        public const string BrokerRedirectUriOnAndroid = "msauth://UserDetailsClient.Droid//{Your package signature}";

        public static string[] Scopes = { "User.Read" };
        public static string Username = string.Empty;

        public static object ParentWindow { get; set; }

        public App()
        {
            PCA = PublicClientApplicationBuilder.Create(ClientID)
                .WithRedirectUri($"msal{ClientID}://auth")
                .Build();

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
