// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using MauiB2C.MSALClient;
using Microsoft.Identity.Client;
using System.Text;

namespace MauiB2C;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        try
        {
            // First attempt silent login, which checks the cache for an existing valid token.
            // If this is very first time or user has signed out, it will throw MsalUiRequiredException
            AuthenticationResult result = await PCAWrapperB2C.Instance.AcquireTokenSilentAsync(B2CConstants.Scopes).ConfigureAwait(false);

            string claims = GetClaims(result);

            // show the claims
            await ShowMessage("AcquireTokenSilent call Claims", claims).ConfigureAwait(false);
        }
        catch (MsalUiRequiredException)
        {
            // This executes UI interaction to obtain token
            AuthenticationResult result = await PCAWrapperB2C.Instance.AcquireTokenInteractiveAsync(B2CConstants.Scopes).ConfigureAwait(false);

            string claims = GetClaims(result);

            // show the Claims
            await ShowMessage("AcquireTokenInteractive call Claims", claims).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await ShowMessage("Exception in AcquireTokenSilent", ex.Message).ConfigureAwait(false);
        }
    }

    private static string GetClaims(AuthenticationResult result)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var claim in result.ClaimsPrincipal.Claims)
        {
            sb.Append("Claim Type = ");
            sb.Append(claim.Type);
            sb.Append("  Value = ");
            sb.AppendLine(claim.Value);
        }

        return sb.ToString();
    }

    private async void SignOutButton_Clicked(object sender, EventArgs e)
    {
        _ = await PCAWrapperB2C.Instance.SignOutAsync().ContinueWith(async (t) =>
        {
            await ShowMessage("Signed Out", "Sign out complete").ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    // display the message
    private Task ShowMessage(string title, string message)
    {
        _ = this.Dispatcher.Dispatch(async () =>
        {
            await DisplayAlert(title, message, "OK").ConfigureAwait(false);
        });

        return Task.CompletedTask;
    }
}

