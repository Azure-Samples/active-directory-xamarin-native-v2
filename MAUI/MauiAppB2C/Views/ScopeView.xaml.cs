using MauiB2C.MSALClient;
using Microsoft.Identity.Client;

namespace MauiB2C.Views;

public partial class ScopeView : ContentPage
{
    public ScopeView()
    {
        InitializeComponent();

        _ = SetClaimsAsync();
    }

    private async Task SetClaimsAsync()
    {
        try
        {
            AuthenticationResult result = await PCAWrapperB2C.Instance.AcquireTokenSilentAsync();

            var name = result.ClaimsPrincipal.FindFirst("name");
            var trustFrameworkPolicy = result.ClaimsPrincipal.FindFirst("tfp");
            var issuedAt = result.ClaimsPrincipal.FindFirst("iat");
            var expiresAt = result.ClaimsPrincipal.FindFirst("exp");

            UserName.Text = name.Value;
            TrustFrameworkPolicy.Text = trustFrameworkPolicy.Value;

            IssuedAt.Text = DateTimeOffset.FromUnixTimeSeconds((long)Convert.ToDouble(issuedAt.Value))
                .DateTime
                .ToLocalTime()
                .ToString();

            ExpiresAt.Text = DateTimeOffset.FromUnixTimeSeconds((long)Convert.ToDouble(expiresAt.Value))
                .DateTime
                .ToLocalTime()
                .ToString();
        }
        catch (MsalUiRequiredException)
        {
            await Shell.Current.GoToAsync("scopeview");
        }
    }

    protected override bool OnBackButtonPressed() { return true; }

    private async void SignOutButton_Clicked(object sender, EventArgs e)
    {
        await PCAWrapperB2C.Instance.SignOutAsync().ContinueWith((t) =>
        {
            return Task.CompletedTask;
        });

        await Shell.Current.GoToAsync("mainview");
    }
}