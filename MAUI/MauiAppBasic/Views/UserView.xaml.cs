using MauiAppBasic.MSALClient;
using MauiAppBasic.ViewModels;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace MauiAppBasic.Views;

public partial class UserView : ContentPage
{
    private UserViewModel _userViewModel = new UserViewModel();
    public UserView()
    {
        BindingContext = _userViewModel;
        _ = GetUserInformationAsync();

        InitializeComponent();
    }

    private async Task GetUserInformationAsync()
    {
        try
        {
            // call Web API to get the data
            AuthenticationResult result = await PCAWrapper.Instance.AcquireTokenSilentAsync(AppConstants.Scopes);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

            GraphServiceClient graphServiceClient = new GraphServiceClient(client);

            var user = await graphServiceClient.Me.GetAsync();

            _userViewModel.UserImage = ImageSource.FromStream(async _ => await graphServiceClient.Me.Photo.Content.GetAsync());
            _userViewModel.DisplayName = user.DisplayName;
            _userViewModel.Email = user.Mail;
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