using MAUI.MSALClient;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace MauiAppBasic.Views;

public partial class UserView : ContentPage
{
    private MSALClientHelper MSALClientHelper;
    private MSGraphHelper MSGraphHelper;
    private const string embeddedConfigFile = "MauiAppBasic.appsettings.json";

    public UserView()
    {
        InitializeComponent();

        this.MSALClientHelper = new MSALClientHelper(embeddedConfigFile);
        this.MSGraphHelper = new MSGraphHelper(this.MSALClientHelper, embeddedConfigFile);

        // Initializes the Public Client app and loads any already signed in user from the token cache
        var cachedUserAccount = Task.Run(async () => await MSALClientHelper.InitializePublicClientAppAsync()).Result;

        _ = GetUserInformationAsync();
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