using MauiAppWithBroker.MSALClient;
using MauiAppWithBroker.ViewModels;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace MauiAppWithBroker.Views;

public partial class UserView : ContentPage
{
    public UserView()
    {
        _ = GetUserInformationAsync();

        InitializeComponent();
    }

    private async Task GetUserInformationAsync()
    {
        try
        {
            // call Web API to get the data
            AuthenticationResult result = await PCAWrapper.Instance.AcquireTokenSilentAsync(PCAWrapper.Instance.GetScopes());

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

            GraphServiceClient graphServiceClient = new GraphServiceClient(client);

            var user = await graphServiceClient.Me.GetAsync();

            UserImage.Source = ImageSource.FromStream(async _ => await graphServiceClient.Me.Photo.Content.GetAsync());
            DisplayName.Text = user.DisplayName;
            Email.Text = user.Mail;
        }
        catch (MsalUiRequiredException)
        {
            await PCAWrapper.Instance.SignOutAsync().ContinueWith((t) =>
            {
                return Task.CompletedTask;
            });

            await Shell.Current.GoToAsync("mainview");
        }
    }

    protected override bool OnBackButtonPressed() { return true; }

    private async void SignOutButton_Clicked(object sender, EventArgs e)
    {
        await PCAWrapper.Instance.SignOutAsync().ContinueWith((t) =>
        {
            return Task.CompletedTask;
        });

        await Shell.Current.GoToAsync("mainview");
    }
}