using MauiAppBasic.MSALClient;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace MauiAppBasic.Views;

public partial class UserView : ContentPage
{
    public UserView()
    {
        InitializeComponent();

        _ = GetUserInformationAsync();
    }

    private async Task GetUserInformationAsync()
    {
        try
        {
            // call Web API to get the data
            AuthenticationResult result = await PublicClientWrapper.Instance.AcquireTokenSilentAsync();

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
            await PublicClientWrapper.Instance.SignOutAsync().ContinueWith((t) =>
            {
                return Task.CompletedTask;
            });

            await Shell.Current.GoToAsync("mainview");
        }
    }

    protected override bool OnBackButtonPressed() { return true; }

    private async void SignOutButton_Clicked(object sender, EventArgs e)
    {
        await PublicClientWrapper.Instance.SignOutAsync().ContinueWith((t) =>
        {
            return Task.CompletedTask;
        });

        await Shell.Current.GoToAsync("mainview");
    }
}