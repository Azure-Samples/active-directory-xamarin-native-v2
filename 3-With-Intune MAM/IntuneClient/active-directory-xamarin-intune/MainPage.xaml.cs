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
        internal static string[] Scopes = { "https://<TODO - Your tenant name>.sharepoint.com/AllSites.Read" }; // TODO - change scopes per your enterprise app

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
                result = await PCAWrapper.Instance.AcquireTokenSilentAsync(Scopes).ConfigureAwait(false);

                await ShowMessage("First AcquireTokenTokenSilent call", result.AccessToken).ConfigureAwait(false);
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    // This executes UI interaction ot obtain token
                    result = await PCAWrapper.Instance.AcquireTokenInteractiveAsync(Scopes).ConfigureAwait(false);

                    await ShowMessage("First AcquireTokenInteractive call", result.AccessToken).ConfigureAwait(false);
                }
                catch (IntuneAppProtectionPolicyRequiredException exProtection)
                {
                    // if the scope requires App Protection Policy,  IntuneAppProtectionPolicyRequiredException is thrown.
                    // Perform registration operation here and then does the silent token acquisition
                    var intuneConnector = DependencyService.Get<IIntuneMAMConnector>(DependencyFetchTarget.GlobalInstance);
                    await intuneConnector.DoMAMRegisterAsync(exProtection).ContinueWith(async (arg) =>
                    {
                        // Now the device is registered, perform token acquisition
                        try
                        {
                            // if no MFA Policy is present, the silent should work.
                            result = await PCAWrapper.Instance.AcquireTokenSilentAsync(Scopes).ConfigureAwait(false);

                            await ShowMessage("AcquireTokenTokenSilent call after Intune registration.", result.AccessToken).ConfigureAwait(false);
                        }
                        catch (MsalUiRequiredException )
                        {
                            // if MFA policy is present, one needs to AcquireTokenInteractive API
                            result = await PCAWrapper.Instance.AcquireTokenInteractiveAsync(Scopes).ConfigureAwait(false);

                            await ShowMessage("Second AcquireTokenInteractive call after Intune registration.", result.AccessToken).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            await ShowMessage("Exception in AcquireTokenSilentAsync after registration.", ex.Message).ConfigureAwait(false);
                        }
                    }).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await ShowMessage("Exception in AcquireTokenInteractiveAsync", ex.Message).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                await ShowMessage("Exception in AcquireTokenTokenSilent", ex.Message).ConfigureAwait(false);
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
            var intuneConnector = DependencyService.Get<IIntuneMAMConnector>(DependencyFetchTarget.GlobalInstance);
            intuneConnector.Unenroll();
        }
    }
}
