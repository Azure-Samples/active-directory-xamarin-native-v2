using MAUI.MSALClient;
//using MauiAppWithBroker.ViewModels;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace MauiAppWithBroker.Views;

public partial class UserView : ContentPage
{
    private MSALClientHelper MSALClientHelper;
    private MSGraphHelper MSGraphHelper;

    public UserView()
    {
        this.MSALClientHelper = new MSALClientHelper();
        this.MSGraphHelper = new MSGraphHelper(this.MSALClientHelper);

        // Initializes the Public Client app and loads any already signed in user from the token cache
        var cachedUserAccount = Task.Run(async () => await MSALClientHelper.InitializePublicClientAppForWAMBrokerAsync()).Result;

        _ = GetUserInformationAsync();

        InitializeComponent();
    }

    private async Task GetUserInformationAsync()
    {
        try
        {
            var user = await this.MSGraphHelper.GetMeAsync();
            UserImage.Source = ImageSource.FromStream(async _ => await this.MSGraphHelper.GetMyPhotoAsync());

            //// call Web API to get the data
            //AuthenticationResult result = await PublicClientWrapper.Instance.AcquireTokenSilentAsync();

            //HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

            //GraphServiceClient graphServiceClient = new GraphServiceClient(client);

            //var user = await graphServiceClient.Me.GetAsync();

            //UserImage.Source = ImageSource.FromStream(async _ => await graphServiceClient.Me.Photo.Content.GetAsync());
            DisplayName.Text = user.DisplayName;
            Email.Text = user.Mail;
        }
        catch (MsalUiRequiredException)
        {
            this.MSALClientHelper.SignOutUser();
            //await PublicClientWrapper.Instance.SignOutAsync().ContinueWith((t) =>
            //{
            //    return Task.CompletedTask;
            //});

            await Shell.Current.GoToAsync("mainview");
        }
    }

    protected override bool OnBackButtonPressed() { return true; }

    private async void SignOutButton_Clicked(object sender, EventArgs e)
    {
        this.MSALClientHelper.SignOutUser();
        //await PublicClientWrapper.Instance.SignOutAsync().ContinueWith((t) =>
        //{
        //    return Task.CompletedTask;
        //});

        await Shell.Current.GoToAsync("mainview");
    }
}