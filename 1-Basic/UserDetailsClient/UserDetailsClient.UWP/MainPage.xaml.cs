using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UserDetailsClient.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            // To get SSO with a UWP app, you'll need to register the following
            // redirect URI for your application
            Uri redirectURIForSsoWithoutBroker = Windows.Security.Authentication.Web.WebAuthenticationBroker.GetCurrentApplicationCallbackUri();

            // To have WAM working you need to register the following redirect URI for your application
            string sid = redirectURIForSsoWithoutBroker.Host.ToUpper();
            
            // only used in the .WithBroker scenario.
            string redirectUriWithWAM = $"ms-appx-web://microsoft.aad.brokerplugin/{sid}";

            // Then use the following:
            LoadApplication(new UserDetailsClient.App(redirectURIForSsoWithoutBroker.AbsoluteUri));
        }
    }
}
