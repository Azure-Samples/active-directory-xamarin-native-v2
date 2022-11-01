using MauiB2C.MSALClient;
using MauiB2C.ViewModels;
using Microsoft.Identity.Client;

namespace MauiB2C.Views;

public partial class ScopeView : ContentPage
{
    private ScopeViewModel _scopeViewModel = new ScopeViewModel();
    public ScopeView()
    {
        BindingContext = _scopeViewModel;

        _ = SetClaimsAsync();
        InitializeComponent();
    }

    private async Task SetClaimsAsync()
    {
        try
        {
            AuthenticationResult result = await PCAWrapperB2C.Instance.AcquireTokenSilentAsync(PCAWrapperB2C.Instance.GetScopes()).ConfigureAwait(false);

            var name = result.ClaimsPrincipal.FindFirst("name");
            var trustFrameworkPolicy = result.ClaimsPrincipal.FindFirst("tfp");
            var issuedAt = result.ClaimsPrincipal.FindFirst("iat");
            var expiresAt = result.ClaimsPrincipal.FindFirst("exp");


            _scopeViewModel.Name = name.Value;
            _scopeViewModel.TrustFrameworkPolicy = trustFrameworkPolicy.Value;
            _scopeViewModel.IssuedAt = DateTimeOffset.FromUnixTimeMilliseconds((long)Convert.ToDouble(issuedAt.Value) * 1000).DateTime.ToLocalTime();
            _scopeViewModel.ExpiresAt = DateTimeOffset.FromUnixTimeMilliseconds((long)Convert.ToDouble(expiresAt.Value) * 1000).DateTime.ToLocalTime();
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