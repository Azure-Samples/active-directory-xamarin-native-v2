using MAUI.MSALClient;
using Microsoft.Extensions.Configuration;
//using MauiAppWithBroker.ViewModels;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Reflection;

namespace MauiAppWithBroker.Views;

public partial class UserView : ContentPage
{
    private MSALClientHelper MSALClientHelper;
    private MSGraphHelper MSGraphHelper;

    public UserView()
    {
        InitializeComponent();

        //var assembly = Assembly.GetExecutingAssembly();
        //using var stream = assembly.GetManifestResourceStream("MauiAppWithBroker.appsettings.json");
        //IConfiguration AppConfiguration = new ConfigurationBuilder()
        //    .AddJsonStream(stream)
        //    .Build();

        //AzureADConfig azureADConfig = AppConfiguration.GetSection("AzureAD").Get<AzureADConfig>();
        //this.MSALClientHelper = new MSALClientHelper(azureADConfig);
        //MSGraphApiConfig graphApiConfig = AppConfiguration.GetSection("MSGraphApi").Get<MSGraphApiConfig>();
        //this.MSGraphHelper = new MSGraphHelper(graphApiConfig, this.MSALClientHelper);

        //// Initializes the Public Client app and loads any already signed in user from the token cache
        //var cachedUserAccount = Task.Run(async () => await MSALClientHelper.InitializePublicClientAppForWAMBrokerAsync()).Result;

        _ = GetUserInformationAsync();
    }

    private async Task GetUserInformationAsync()
    {
        try
        {
            var user = await PublicClientSingleton.Instance.MSGraphHelper.GetMeAsync();
            UserImage.Source = ImageSource.FromStream(async _ => await PublicClientSingleton.Instance.MSGraphHelper.GetMyPhotoAsync());

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
            await PublicClientSingleton.Instance.SignOutAsync();
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
        await PublicClientSingleton.Instance.SignOutAsync();
        //await PublicClientWrapper.Instance.SignOutAsync().ContinueWith((t) =>
        //{
        //    return Task.CompletedTask;
        //});

        await Shell.Current.GoToAsync("mainview");
    }
}