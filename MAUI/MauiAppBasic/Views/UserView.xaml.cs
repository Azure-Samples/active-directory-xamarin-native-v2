using MAUI.MSALClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Reflection;

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
            var user = await PublicClientSingleton.Instance.MSGraphHelper.GetMeAsync();
            var userPhoto =  ImageSource.FromStream(async _ => await PublicClientSingleton.Instance.MSGraphHelper.GetMyPhotoAsync());

            if (userPhoto is not null)
            {
                UserImage.Source = userPhoto;
            }

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