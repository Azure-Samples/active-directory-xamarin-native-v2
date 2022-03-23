// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using activedirectoryxamarinintune;
using Microsoft.Identity.Client;
using Xamarin.Forms;

namespace active_directory_xamarin_intune
{
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// The scopes that are protected by conditional access
        /// </summary>
        internal static string[] Scopes = { "api://xam-Intune-sample-EnterpriseApp/Hello.World" }; // TODO - change scopes are per your enterprise app

        public MainPage()
        {
            InitializeComponent();
        }

        async void btnAcquireToken_Clicked(System.Object sender, System.EventArgs e)
        {
            AuthenticationResult result = null;

            try
            {
                // attempt silent login.
                // If this is very first time and the device is not enrolled, it will throw MsalUiRequiredException
                // If the device is enrolled, this will succeed.
                result = await PCAWrapper.Instance.DoSilentAsync(Scopes).ConfigureAwait(false);

                await ShowMessage("Silent 1", result.AccessToken).ConfigureAwait(false);
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    // This executes UI interaction ot obtain token
                    result = await PCAWrapper.Instance.DoInteractiveAsync(Scopes).ConfigureAwait(false);

                    await ShowMessage("Interactive 1", result.AccessToken).ConfigureAwait(false);
                }
                catch (IntuneAppProtectionPolicyRequiredException exProtection)
                {
                    // if the scope requires App Protection Policy,  IntuneAppProtectionPolicyRequiredException is thrown.
                    // Perform registration operation here and then does the silent token acquisition
                    var intuneConnector = DependencyService.Get<IIntuneMAMConnector>(DependencyFetchTarget.GlobalInstance);
                    await intuneConnector.DoMAMRegister(exProtection).ContinueWith(async (arg) =>
                    {
                        try
                        {
                            // Now the device is registered, perform silent token acquisition
                            result = await PCAWrapper.Instance.DoSilentAsync(Scopes).ConfigureAwait(false);

                            await ShowMessage("Silent 2", result.AccessToken).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            await ShowMessage("Exception 1", ex.Message).ConfigureAwait(false);
                        }
                    }).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                await ShowMessage("Exception 2", ex.Message).ConfigureAwait(false);
            }
        }

        // display the message
        private Task ShowMessage(string title, string message)
        {
            Dispatcher.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert(title, message, "OK").ConfigureAwait(false);
            });

            return Task.CompletedTask;
        }

        // called when signout it pressed
        async void btnSignOut_Clicked(System.Object sender, System.EventArgs e)
        {
            await PCAWrapper.Instance.SignOut().ConfigureAwait(false);
        }
    }
}
